using Microsoft.Xna.Framework;

namespace ParaTH;

using DepthBuckets = UnsafePooledList<UnsafePooledList<Entity>>;

public sealed class HierarchySystem(World world) : IDisposable
{
    private readonly World world = world;
    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Hierarchy>();

    private readonly DepthBuckets childrenEntityBuckets = new();
    private int maxDepthSeen = -1;

    public void Update()
    {
        var buckets = childrenEntityBuckets;

        var q = world.GetOrCreateQuery(descriptor);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<Hierarchy>(out var local);
                var entities = chunk.Entities;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    int depth = local.UnsafeAt(i).Depth;

                    if (depth > maxDepthSeen)
                    {
                        for (int d = maxDepthSeen + 1; d <= depth; d++)
                            buckets.Add(new UnsafePooledList<Entity>(64));

                        maxDepthSeen = depth;
                    }

                    // sort all children entities in depth order for proper hierarchy handling
                    buckets[depth].Add(entities.UnsafeAt(i));
                }
            }
        }

        // todo: this is really bad, optimize if profiler tells us to
        // all 3 GetComponents are random memory accesses happening in (fairly)hot loops
        // maybe store transforms in buckets as well & use its sorted index for accessing?
        // it's still random access but will fit into L1 cache better
        for (int depth = 0; depth <= maxDepthSeen; depth++)
        {
            var bucket = buckets[depth];
            var bucketSpan = bucket.AsSpan();

            for (int i = 0; i < bucketSpan.Length; i++)
            {
                var entity = bucketSpan.UnsafeAt(i);
                ref var local = ref world.GetComponent<Hierarchy>(entity);

                // parents should always outlive their children
                //if (!world.IsAlive(local.Parent))
                //    continue;

                ref var childTransform = ref world.GetComponent<Transform>(entity);
                ref var parentTransform = ref world.GetComponent<Transform>(local.Parent);

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
