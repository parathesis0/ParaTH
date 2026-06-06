using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace ParaTH;

// handles hierarchy transform, children's transform are based off its parents
[SkipLocalsInit]
public sealed class HierarchySystem(World world) : IDisposable
{
    private readonly World world = world;
    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Hierarchy>();

    private readonly UnsafePooledList<StackNode> traversalStack = new(64);

    // a parent whose children still need their world transform written. carrying the
    // already-resolved world Transform + FirstChild here means a node is never re-resolved
    // through the EntityDataMap when it is popped: it was fully resolved as a child already.
    // pure value type (no managed refs) so the pooled stack never has to clear slots.
    private readonly struct StackNode(Entity firstChild, Transform world)
    {
        public readonly Entity FirstChild = firstChild;
        public readonly Transform World = world;
    }

    public void Update()
    {
        var q = world.GetOrCreateQuery(descriptor);
        var stack = traversalStack;

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<Hierarchy>(out var hierarchies);
                chunk.GetFilledComponentSpan<Transform>(out var transforms);

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var hierarchy = ref hierarchies.UnsafeAt(i);
                    if (hierarchy.Parent != default || hierarchy.FirstChild == default)
                        continue;

                    // seed the root straight from the chunk span — sequential, no random lookup.
                    stack.Clear();
                    stack.Add(new StackNode(hierarchy.FirstChild, transforms.UnsafeAt(i)));
                    PropagateSubtree(stack);
                }
            }
        }
    }

    private void PropagateSubtree(UnsafePooledList<StackNode> stack)
    {
        while (stack.Count > 0)
        {
            var node = stack[^1];
            stack.RemoveLast();

            float cos = MathF.Cos(node.World.Rotation);
            float sin = MathF.Sin(node.World.Rotation);

            Entity child = node.FirstChild;
            while (child != default)
            {
                ref var childHierarchy = ref world.GetComponent<Hierarchy>(child);
                Entity next = childHierarchy.NextSibling;

                ref var childTransform = ref world.GetComponent<Transform>(child);
                ApplyParentTransform(in node.World, ref childHierarchy, ref childTransform, cos, sin);

                // push the child's freshly-written world transform; no re-resolve on pop.
                if (childHierarchy.FirstChild != default)
                    stack.Add(new StackNode(childHierarchy.FirstChild, childTransform));

                child = next;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ApplyParentTransform(
#pragma warning disable RCS1242 // Do not pass non-read-only struct by read-only reference
        in Transform parentTransform, ref Hierarchy local, ref Transform childTransform, float cos, float sin)
#pragma warning restore RCS1242 // Do not pass non-read-only struct by read-only reference
    {
        float localX = local.LocalPosition.X * parentTransform.Scale.X;
        float localY = local.LocalPosition.Y * parentTransform.Scale.Y;

        childTransform.Position = new Vector2(
            parentTransform.Position.X + (localX * cos - localY * sin),
            parentTransform.Position.Y + (localX * sin + localY * cos));

        if (!local.PreserveTransformRotation)
            childTransform.Rotation = parentTransform.Rotation + local.LocalRotation;

        childTransform.Scale = parentTransform.Scale * local.LocalScale;
    }

    public void TrimExcess()
    {
        traversalStack.Clear();
    }

    public void Dispose()
    {
        traversalStack.Dispose();
    }
}
