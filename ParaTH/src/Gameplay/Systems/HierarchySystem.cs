using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace ParaTH;

using DepthBuckets = UnsafePooledList<UnsafePooledList<HierarchySystem.ChildSlot>>;

// handles hierarchy transform, children's transform are based off its parents
[SkipLocalsInit]
public sealed class HierarchySystem(World world) : IDisposable
{
    // compact handle to a child's location in the archetype chunk storage
    // avoids EntityDataMap round-trip for the child's own Hierarchy+Transform
    public struct ChildSlot
    {
        public Archetype Archetype;
        public int ChunkIndex;
        public int Index;
        public Entity Parent;
    }

    private readonly World world = world;
    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Hierarchy>();

    private readonly DepthBuckets childrenEntityBuckets = new();
    private int maxDepthSeen = -1;

    public void Update()
    {
        var buckets = childrenEntityBuckets;

        var q = world.GetOrCreateQuery(descriptor);

        // pass 1: bucket entities by depth, capturing their chunk location
        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            var chunks = archetype.GetChunksSpan();
            for (int ci = 0; ci < chunks.Length; ci++)
            {
                ref var chunk = ref chunks.UnsafeAt(ci);
                chunk.GetFilledComponentSpan<Hierarchy>(out var hierarchies);

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    int depth = hierarchies.UnsafeAt(i).Depth;

                    if (depth > maxDepthSeen)
                    {
                        for (int d = maxDepthSeen + 1; d <= depth; d++)
                            buckets.Add(new UnsafePooledList<ChildSlot>(64));

                        maxDepthSeen = depth;
                    }

                    buckets[depth].Add(new ChildSlot
                    {
                        Archetype = archetype,
                        ChunkIndex = ci,
                        Index = i,
                        Parent = hierarchies.UnsafeAt(i).Parent
                    });
                }
            }
        }

        // pass 2: propagate transforms top-down
        // only the parent Transform lookup is a true random access (via World.GetComponent)
        // the child's Hierarchy+Transform are accessed directly from the stored chunk location
        for (int depth = 0; depth <= maxDepthSeen; depth++)
        {
            var bucket = buckets[depth];
            var bucketSpan = bucket.AsSpan();

            for (int i = 0; i < bucketSpan.Length; i++)
            {
                ref var slot = ref bucketSpan.UnsafeAt(i);

                // direct chunk access — no EntityDataMap lookup
                ref var chunk = ref slot.Archetype.GetChunk(slot.ChunkIndex);
                ref var local = ref chunk.Get<Hierarchy>(slot.Index);
                ref var childTransform = ref chunk.Get<Transform>(slot.Index);

                // only random access: parent's transform
                ref var parentTransform = ref world.GetComponent<Transform>(slot.Parent);

                // apply parent's transform
                float cos = MathF.Cos(parentTransform.Rotation);
                float sin = MathF.Sin(parentTransform.Rotation);

                float localX = local.LocalPosition.X * parentTransform.Scale.X;
                float localY = local.LocalPosition.Y * parentTransform.Scale.Y;

                childTransform.Position = new Vector2(
                    parentTransform.Position.X + (localX * cos - localY * sin),
                    parentTransform.Position.Y + (localX * sin + localY * cos));

                childTransform.Rotation = parentTransform.Rotation + local.LocalRotation;
                childTransform.Scale = parentTransform.Scale * local.LocalScale;
            }

            bucket.Clear();
        }
    }

    public void TrimExcess()
    {
        for (int i = 0; i < childrenEntityBuckets.Count; i++)
            childrenEntityBuckets[i].Dispose();

        childrenEntityBuckets.Clear();
        maxDepthSeen = -1;
    }

    public void Dispose()
    {
        for (int i = 0; i < childrenEntityBuckets.Count; i++)
            childrenEntityBuckets[i].Dispose();

        childrenEntityBuckets.Dispose();
    }
}
