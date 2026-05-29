using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace ParaTH;

// Unity-like runtime parenting on top of the Hierarchy component.
//
// the Hierarchy component stores the child -> parent link + local TRS and is what
// HierarchySystem/LifetimeSystem/MovementSystem already consume. it has no parent -> child link,
// which is needed to (a) walk a subtree to fix Depth after a reparent and (b) enumerate children.
// this manager owns that missing adjacency externally (keyed by entity id) so the component layout
// and the three systems stay untouched.
//
// structural moves (Add/Remove<Hierarchy>) happen here, so do NOT call SetParent/Unparent while a
// system is mid-iteration over archetypes. call it from script/builder/gameplay code, same as entity
// creation. destruction is left to LifetimeSystem's group-life semantics (a parent and its children
// die as a unit); the manager does not prune links on destroy.
[SkipLocalsInit]
public sealed class HierarchyManager(World world) : IDisposable
{
    // intrusive sibling linked list, one entry per entity that participates in a hierarchy.
    // default(Entity) (PackedValue 0, Version 0) is the null sentinel — live entities always have Version >= 1.
    private struct Links
    {
        public Entity FirstChild;
        public Entity NextSibling;
        public Entity PrevSibling;
        public int ChildCount;
    }

    private readonly World world = world;
    private readonly SparsePagedArray<Links> links = new(pageSize: 256);
    private readonly UnsafePooledList<Entity> depthFixStack = new(64);

    #region Public API
    // make `parent` the parent of `child`. pass default(Entity) for parent to unparent (-> root).
    // worldPositionStays: true keeps the child's world transform (local TRS is recomputed from world,
    // Unity's default); false keeps the child's local TRS (its world transform jumps under the new parent).
    public void SetParent(Entity child, Entity parent, bool worldPositionStays = true)
    {
        Debug.Assert(world.IsAlive(child));

        if (parent == default)
        {
            Unparent(child, worldPositionStays);
            return;
        }

        Debug.Assert(world.IsAlive(parent));
        Debug.Assert(child != parent, "an entity cannot parent itself.");
        Debug.Assert(!WouldCreateCycle(child, parent), "SetParent would create a cycle.");

        EnsureLinkCapacity();

        // detach from the current parent (if any) before relinking.
        Entity oldParent = GetParent(child);
        if (oldParent == parent)
        {
            // same parent: just refresh local TRS per the requested mode, no relink/depth change.
            ref var existing = ref world.GetComponent<Hierarchy>(child);
            WriteLocalTransform(ref existing, child, parent, worldPositionStays, hadHierarchy: true);
            return;
        }
        if (oldParent != default)
            UnlinkChild(oldParent, child);

        int parentDepth = world.HasComponent<Hierarchy>(parent)
            ? world.GetComponent<Hierarchy>(parent).Depth
            : 0;
        int childDepth = parentDepth + 1;

        bool hadHierarchy = world.HasComponent<Hierarchy>(child);
        if (hadHierarchy)
        {
            ref var hierarchy = ref world.GetComponent<Hierarchy>(child);
            hierarchy.Parent = parent;
            hierarchy.Depth = childDepth;
            WriteLocalTransform(ref hierarchy, child, parent, worldPositionStays, hadHierarchy: true);
        }
        else
        {
            var hierarchy = new Hierarchy(parent, Vector2.Zero, Vector2.One, 0) { Depth = childDepth };
            WriteLocalTransform(ref hierarchy, child, parent, worldPositionStays, hadHierarchy: false);
            world.AddComponent(child, hierarchy); // structural: moves child to a +Hierarchy archetype
        }

        LinkChild(parent, child);
        FixSubtreeDepth(child);
    }

    // detach `child` from its parent, making it a root. removes the Hierarchy component.
    // the child's Transform already holds its world transform (HierarchySystem keeps it baked),
    // so its on-screen pose is preserved regardless of worldPositionStays.
    public void Unparent(Entity child, bool worldPositionStays = true)
    {
        Debug.Assert(world.IsAlive(child));
        _ = worldPositionStays; // world transform is always already baked into Transform; kept for API symmetry

        if (!world.HasComponent<Hierarchy>(child))
            return;

        Entity oldParent = world.GetComponent<Hierarchy>(child).Parent;
        if (oldParent != default)
            UnlinkChild(oldParent, child);

        // children of `child` are now rooted under a node that loses its own parent link.
        // its subtree depths shift up by one level relative to it; recompute from depth 0.
        ref var childLinks = ref links.TryGetRef(child.Id);
        bool hasChildren = !Unsafe.IsNullRef(ref childLinks) && childLinks.FirstChild != default;

        world.RemoveComponent<Hierarchy>(child); // structural: moves child to a -Hierarchy archetype

        if (hasChildren)
            FixSubtreeDepthFromRoot(child);
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
        ref var l = ref links.TryGetRef(parent.Id);
        return Unsafe.IsNullRef(ref l) ? 0 : l.ChildCount;
    }

    public Entity GetFirstChild(Entity parent)
    {
        ref var l = ref links.TryGetRef(parent.Id);
        return Unsafe.IsNullRef(ref l) ? default : l.FirstChild;
    }

    public Entity GetNextSibling(Entity child)
    {
        ref var l = ref links.TryGetRef(child.Id);
        return Unsafe.IsNullRef(ref l) ? default : l.NextSibling;
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
        ref Hierarchy hierarchy, Entity child, Entity parent, bool worldPositionStays, bool hadHierarchy)
    {
        if (worldPositionStays)
        {
            ref var childTransform = ref world.GetComponent<Transform>(child);
            ref var parentTransform = ref world.GetComponent<Transform>(parent);
            LocalFromWorld(in parentTransform, in childTransform, ref hierarchy);
        }
        else if (!hadHierarchy)
        {
            // was a root: its current Transform is its world == its new local (world will jump).
            ref var childTransform = ref world.GetComponent<Transform>(child);
            hierarchy.LocalPosition = childTransform.Position;
            hierarchy.LocalScale = childTransform.Scale;
            hierarchy.LocalRotation = childTransform.Rotation;
        }
        // else: already had a Hierarchy and worldPositionStays == false -> keep existing local TRS untouched.
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
    // re-derive depths for the whole subtree rooted at `node`, whose own Depth is already finalized.
    private void FixSubtreeDepth(Entity node)
    {
        var stack = depthFixStack;
        stack.Clear();
        stack.Add(node);

        while (stack.Count > 0)
        {
            var current = stack[stack.Count - 1];
            stack.RemoveLast();

            int currentDepth = world.GetComponent<Hierarchy>(current).Depth;

            ref var l = ref links.TryGetRef(current.Id);
            if (Unsafe.IsNullRef(ref l))
                continue;

            var c = l.FirstChild;
            while (c != default)
            {
                ref var childHierarchy = ref world.GetComponent<Hierarchy>(c);
                childHierarchy.Depth = currentDepth + 1;
                stack.Add(c);
                c = links[c.Id].NextSibling;
            }
        }
    }

    // `node` just lost its Hierarchy (it is now a root at depth 0). its children become depth 0 roots-of-subtree
    // re-anchored: each direct child's depth becomes 0, and the rest cascade. used on Unparent.
    private void FixSubtreeDepthFromRoot(Entity node)
    {
        ref var l = ref links.TryGetRef(node.Id);
        if (Unsafe.IsNullRef(ref l))
            return;

        var c = l.FirstChild;
        while (c != default)
        {
            ref var childHierarchy = ref world.GetComponent<Hierarchy>(c);
            childHierarchy.Depth = 0;
            FixSubtreeDepth(c);
            c = links[c.Id].NextSibling;
        }
    }
    #endregion

    #region Link maintenance
    // push `child` to the front of `parent`'s child list. both ids are assumed in-capacity.
    private void LinkChild(Entity parent, Entity child)
    {
        EnsureLinkSlot(parent.Id);
        EnsureLinkSlot(child.Id);

        ref var parentLinks = ref links[parent.Id];
        ref var childLinks = ref links[child.Id];

        Entity oldFirst = parentLinks.FirstChild;

        childLinks.PrevSibling = default;
        childLinks.NextSibling = oldFirst;
        if (oldFirst != default)
            links[oldFirst.Id].PrevSibling = child;

        parentLinks.FirstChild = child;
        parentLinks.ChildCount++;
    }

    private void UnlinkChild(Entity parent, Entity child)
    {
        ref var childLinks = ref links.TryGetRef(child.Id);
        if (Unsafe.IsNullRef(ref childLinks))
            return;

        Entity prev = childLinks.PrevSibling;
        Entity next = childLinks.NextSibling;

        if (prev != default)
            links[prev.Id].NextSibling = next;
        else
        {
            ref var parentLinks = ref links.TryGetRef(parent.Id);
            if (!Unsafe.IsNullRef(ref parentLinks))
                parentLinks.FirstChild = next;
        }

        if (next != default)
            links[next.Id].PrevSibling = prev;

        childLinks.PrevSibling = default;
        childLinks.NextSibling = default;

        ref var pLinks = ref links.TryGetRef(parent.Id);
        if (!Unsafe.IsNullRef(ref pLinks) && pLinks.ChildCount > 0)
            pLinks.ChildCount--;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureLinkSlot(int id)
    {
        if (!links.ContainsKey(id))
            links.Add(id, default);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureLinkCapacity()
    {
        int max = world.MaxEntityId;
        if (max >= 0)
            links.EnsureCapacity(max + 1);
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
        links.TrimExcess();
    }

    public void Dispose()
    {
        depthFixStack.Dispose();
    }
}
