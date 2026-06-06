using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace ParaTH;

// Unity-like runtime parenting on top of the Hierarchy component.
//
// Hierarchy is now an intrusive node: the same component stores child -> parent
// local TRS and parent -> children sibling links. This changes the component
// semantic from "has a parent" to "has a parent or has children".
//
// structural moves (Add/Remove<Hierarchy>) happen here, so do NOT call SetParent/Unparent while a
// system is mid-iteration over archetypes. call it from script/builder/gameplay code, same as entity
// creation. destruction is left to LifetimeSystem's group-life semantics (a parent and its children
// die as a unit); the manager does not prune links on destroy.
[SkipLocalsInit]
public sealed class HierarchyManager(World world) : IDisposable
{
    private readonly World world = world;
    private readonly UnsafePooledList<Entity> depthFixStack = new(64);

    #region Public API
    // make `parent` the parent of `child`. pass default(Entity) for parent to unparent (-> root).
    // worldPositionStays: true keeps the child's world transform (local TRS is recomputed from world,
    // Unity's default); false keeps the child's local TRS (its world transform jumps under the new parent).
    public void SetParent(Entity child, Entity parent, bool worldPositionStays = true,
                          bool preserveTransformRotation = false)
    {
        Debug.Assert(world.IsAlive(child));

        if (parent == default)
        {
            Unparent(child);
            return;
        }

        Debug.Assert(world.IsAlive(parent));
        Debug.Assert(child != parent, "an entity cannot parent itself.");
        Debug.Assert(!WouldCreateCycle(child, parent), "SetParent would create a cycle.");

        Entity oldParent = GetParent(child);
        if (oldParent == parent)
        {
            // same parent: just refresh local TRS per the requested mode, no relink/depth change.
            ref var existing = ref world.GetComponent<Hierarchy>(child);
            existing.PreserveTransformRotation = preserveTransformRotation;
            WriteLocalTransform(ref existing, child, parent, worldPositionStays, hadParent: true);
            return;
        }

        if (oldParent != default)
            UnlinkChild(oldParent, child);

        EnsureHierarchy(parent);
        int childDepth = GetDepthAsChildOf(parent);
        bool hadParent = oldParent != default;

        if (world.HasComponent<Hierarchy>(child))
        {
            ref var hierarchy = ref world.GetComponent<Hierarchy>(child);
            hierarchy.Parent = parent;
            hierarchy.Depth = childDepth;
            hierarchy.PreserveTransformRotation = preserveTransformRotation;
            WriteLocalTransform(ref hierarchy, child, parent, worldPositionStays, hadParent);
        }
        else
        {
            var hierarchy = new Hierarchy(parent, Vector2.Zero, Vector2.One, 0, preserveTransformRotation)
            {
                Depth = childDepth
            };
            WriteLocalTransform(ref hierarchy, child, parent, worldPositionStays, hadParent: false);
            world.AddComponent(child, hierarchy); // structural: moves child to a +Hierarchy archetype
        }

        LinkChild(parent, child);
        FixSubtreeDepth(child);
    }

    // detach `child` from its parent, making it a root. if the node still has children, its Hierarchy
    // component is kept for the intrusive child list; otherwise it is removed.
    public void Unparent(Entity child)
    {
        Debug.Assert(world.IsAlive(child));

        if (!world.HasComponent<Hierarchy>(child))
            return;

        Entity oldParent = world.GetComponent<Hierarchy>(child).Parent;
        if (oldParent == default)
        {
            RemoveHierarchyIfDetachedLeaf(child);
            return;
        }

        UnlinkChild(oldParent, child);

        ref var hierarchy = ref world.GetComponent<Hierarchy>(child);
        hierarchy.Parent = default;
        hierarchy.PrevSibling = default;
        hierarchy.NextSibling = default;
        hierarchy.Depth = 0;

        if (hierarchy.FirstChild != default)
            FixSubtreeDepth(child);
        else
            world.RemoveComponent<Hierarchy>(child); // structural: moves child to a -Hierarchy archetype
    }

    public Entity GetParent(Entity child)
    {
        Debug.Assert(world.IsAlive(child));

        if (world.TryGetComponent<Hierarchy>(child, out var hierarchy))
            return hierarchy.Parent;
        return default;
    }

    public int GetChildCount(Entity parent)
    {
        if (world.TryGetComponent<Hierarchy>(parent, out var hierarchy))
            return hierarchy.ChildCount;
        return 0;
    }

    public Entity GetFirstChild(Entity parent)
    {
        if (world.TryGetComponent<Hierarchy>(parent, out var hierarchy))
            return hierarchy.FirstChild;
        return default;
    }

    public Entity GetNextSibling(Entity child)
    {
        if (world.TryGetComponent<Hierarchy>(child, out var hierarchy))
            return hierarchy.NextSibling;
        return default;
    }

    // O(index) walk. returns default if out of range.
    public Entity GetChild(Entity parent, int index)
    {
        Entity current = GetFirstChild(parent);
        while (current != default && index-- > 0)
            current = GetNextSibling(current);
        return current;
    }

    // allocation-free foreach: `foreach (var c in manager.GetChildren(parent))`
    public ChildEnumerator GetChildren(Entity parent) => new(this, GetFirstChild(parent));

    public struct ChildEnumerator
    {
        private readonly HierarchyManager manager;
        private Entity next;
        public Entity Current { get; private set; }

        internal ChildEnumerator(HierarchyManager manager, Entity firstChild)
        {
            this.manager = manager;
            next = firstChild;
            Current = default;
        }

        public bool MoveNext()
        {
            if (next == default)
                return false;
            Current = next;
            next = manager.GetNextSibling(Current);
            return true;
        }

        public readonly ChildEnumerator GetEnumerator() => this;
    }
    #endregion

    #region Transform math
    // compute and store the child's local TRS for the chosen reparent mode.
    private void WriteLocalTransform(
        ref Hierarchy hierarchy, Entity child, Entity parent, bool worldPositionStays, bool hadParent)
    {
        if (worldPositionStays)
        {
            ref var childTransform = ref world.GetComponent<Transform>(child);
            ref var parentTransform = ref world.GetComponent<Transform>(parent);
            LocalFromWorld(in parentTransform, in childTransform, ref hierarchy);
        }
        else if (!hadParent)
        {
            // was a root: its current Transform is its world == its new local (world will jump).
            ref var childTransform = ref world.GetComponent<Transform>(child);
            hierarchy.LocalPosition = childTransform.Position;
            hierarchy.LocalScale = childTransform.Scale;
            hierarchy.LocalRotation = childTransform.Rotation;
        }
        // else: already had a parent and worldPositionStays == false -> keep existing local TRS untouched.
    }

    // inverse of HierarchySystem's forward transform: recover local TRS that reproduces the child's
    // current world transform under `parent`.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LocalFromWorld(in Transform parent, in Transform child, ref Hierarchy local)
    {
        float cos = MathF.Cos(parent.Rotation);
        float sin = MathF.Sin(parent.Rotation);

        float dx = child.Position.X - parent.Position.X;
        float dy = child.Position.Y - parent.Position.Y;

        // rotate the offset by -parent.Rotation, then undo the parent scale
        float localX = dx * cos + dy * sin;
        float localY = -dx * sin + dy * cos;

        local.LocalPosition = new Vector2(
            SafeDivide(localX, parent.Scale.X),
            SafeDivide(localY, parent.Scale.Y));
        local.LocalScale = new Vector2(
            SafeDivide(child.Scale.X, parent.Scale.X),
            SafeDivide(child.Scale.Y, parent.Scale.Y));
        local.LocalRotation = child.Rotation - parent.Rotation;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float SafeDivide(float a, float b) => b == 0f ? 0f : a / b;
    #endregion

    #region Depth maintenance
    // re-derive depths for the whole subtree rooted at `node`, whose own Depth is already finalized
    // when it has a parent. parentless roots use virtual depth -1, so direct children become depth 0.
    private void FixSubtreeDepth(Entity node)
    {
        var stack = depthFixStack;
        stack.Clear();
        stack.Add(node);

        while (stack.Count > 0)
        {
            var current = stack[^1];
            stack.RemoveLast();

            ref var currentHierarchy = ref world.GetComponent<Hierarchy>(current);
            int currentDepth = currentHierarchy.Parent != default ? currentHierarchy.Depth : -1;

            var c = currentHierarchy.FirstChild;
            while (c != default)
            {
                ref var childHierarchy = ref world.GetComponent<Hierarchy>(c);
                childHierarchy.Depth = currentDepth + 1;
                stack.Add(c);
                c = childHierarchy.NextSibling;
            }
        }
    }
    #endregion

    #region Link maintenance
    // push `child` to the front of `parent`'s child list.
    private void LinkChild(Entity parent, Entity child)
    {
        ref var parentHierarchy = ref world.GetComponent<Hierarchy>(parent);
        Entity oldFirst = parentHierarchy.FirstChild;

        ref var childHierarchy = ref world.GetComponent<Hierarchy>(child);
        childHierarchy.PrevSibling = default;
        childHierarchy.NextSibling = oldFirst;

        if (oldFirst != default)
            world.GetComponent<Hierarchy>(oldFirst).PrevSibling = child;

        parentHierarchy.FirstChild = child;
        parentHierarchy.ChildCount++;
    }

    private void UnlinkChild(Entity parent, Entity child)
    {
        if (!world.HasComponent<Hierarchy>(child))
            return;

        ref var childHierarchy = ref world.GetComponent<Hierarchy>(child);
        Entity prev = childHierarchy.PrevSibling;
        Entity next = childHierarchy.NextSibling;

        if (prev != default)
            world.GetComponent<Hierarchy>(prev).NextSibling = next;
        else if (world.HasComponent<Hierarchy>(parent))
            world.GetComponent<Hierarchy>(parent).FirstChild = next;

        if (next != default)
            world.GetComponent<Hierarchy>(next).PrevSibling = prev;

        childHierarchy.PrevSibling = default;
        childHierarchy.NextSibling = default;

        if (world.HasComponent<Hierarchy>(parent))
        {
            ref var parentHierarchy = ref world.GetComponent<Hierarchy>(parent);
            if (parentHierarchy.ChildCount > 0)
                parentHierarchy.ChildCount--;
        }

        RemoveHierarchyIfDetachedLeaf(parent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref Hierarchy EnsureHierarchy(Entity entity)
    {
        if (!world.HasComponent<Hierarchy>(entity))
            world.AddComponent(entity, new Hierarchy(default, Vector2.Zero, Vector2.One, 0));
        return ref world.GetComponent<Hierarchy>(entity);
    }

    private void RemoveHierarchyIfDetachedLeaf(Entity entity)
    {
        if (!world.HasComponent<Hierarchy>(entity))
            return;

        ref var hierarchy = ref world.GetComponent<Hierarchy>(entity);
        if (hierarchy.Parent == default && hierarchy.FirstChild == default)
            world.RemoveComponent<Hierarchy>(entity);
    }

    private int GetDepthAsChildOf(Entity parent)
    {
        ref var hierarchy = ref world.GetComponent<Hierarchy>(parent);
        return hierarchy.Parent != default ? hierarchy.Depth + 1 : 0;
    }
    #endregion

    private bool WouldCreateCycle(Entity child, Entity parent)
    {
        // walk up from the prospective parent; if we reach `child`, parenting closes a loop.
        Entity cursor = parent;
        while (cursor != default && world.IsAlive(cursor))
        {
            if (cursor == child)
                return true;
            if (!world.TryGetComponent<Hierarchy>(cursor, out var h))
                break;
            cursor = h.Parent;
        }
        return false;
    }

    public void TrimExcess()
    {
        depthFixStack.Clear();
    }

    public void Dispose()
    {
        depthFixStack.Dispose();
    }
}
