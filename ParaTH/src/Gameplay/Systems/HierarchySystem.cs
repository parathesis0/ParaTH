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

    private readonly UnsafePooledList<Entity> traversalStack = new(64);

    public void Update()
    {
        var q = world.GetOrCreateQuery(descriptor);
        var stack = traversalStack;

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<Hierarchy>(out var hierarchies);

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var hierarchy = ref hierarchies.UnsafeAt(i);
                    if (hierarchy.Parent != default || hierarchy.FirstChild == default)
                        continue;

                    stack.Clear();
                    stack.Add(chunk.Entities.UnsafeAt(i));
                    PropagateSubtree(stack);
                }
            }
        }
    }

    private void PropagateSubtree(UnsafePooledList<Entity> stack)
    {
        while (stack.Count > 0)
        {
            var parent = stack[^1];
            stack.RemoveLast();

            ref var parentHierarchy = ref world.GetComponent<Hierarchy>(parent);
            ref var parentTransform = ref world.GetComponent<Transform>(parent);

            float cos = MathF.Cos(parentTransform.Rotation);
            float sin = MathF.Sin(parentTransform.Rotation);

            Entity child = parentHierarchy.FirstChild;
            while (child != default)
            {
                ref var childHierarchy = ref world.GetComponent<Hierarchy>(child);
                Entity next = childHierarchy.NextSibling;

                ref var childTransform = ref world.GetComponent<Transform>(child);
                ApplyParentTransform(in parentTransform, ref childHierarchy, ref childTransform, cos, sin);

                if (childHierarchy.FirstChild != default)
                    stack.Add(child);

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
