using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed class HierarcySystem(World world) : IDisposable
{
    private readonly World world = world;
    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, LocalTransform>();

    private struct HierarcyNode : IComparable<HierarcyNode>
    {
        public Entity Entity;
        public int Depth;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int CompareTo(HierarcyNode other)
        {
            return Depth.CompareTo(other.Depth);
        }
    }

    private readonly UnsafePooledList<HierarcyNode> nodes = new(256);

    public void Update()
    {
        var nodes = this.nodes;
        nodes.Clear();

        var q = world.GetOrCreateQuery(descriptor);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<LocalTransform>(out var local);
                var entities = chunk.Entities;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    nodes.Add(new HierarcyNode
                    {
                        Entity = entities.UnsafeAt(i),
                        Depth = local.UnsafeAt(i).Depth,
                    });
                }
            }
        }

        // we can probably use bucket sort or something
        // given that there likely won't be that many layers
        // reasonably you need like 4 layers at most.
        var nodeSpan = nodes.AsSpan();
        if (nodeSpan.Length > 1)
            nodeSpan.Sort();

        // todo: this is really bad, optimize if profiler tells us to
        // GetComponents and IsAlive are both random memory accesses happening in (fairly)hot loops
        // im thinking of putting all transforms in HierarcyNode & sorting its index for accessing
        // it's still random access but it will fit into the L1 cache better
        // and IsAlive should be checked for each parent once instead of having every child check its parent
        for (int i = 0; i < nodeSpan.Length; i++)
        {
            var entity = nodeSpan.UnsafeAt(i).Entity;
            ref var local = ref world.GetComponent<LocalTransform>(entity);

            // your parents are dead lmao
            // not sure if should even happen
            if (!world.IsAlive(local.Parent))
                continue;

            ref var parentTransform = ref world.GetComponent<Transform>(local.Parent);
            ref var childTransform = ref world.GetComponent<Transform>(entity);

            // apply parent's transform
            float cos = MathF.Cos(parentTransform.Rotation);
            float sin = MathF.Sin(parentTransform.Rotation);

            float localX = local.LocalPosition.X * parentTransform.Scale.X;
            float localY = local.LocalPosition.Y * parentTransform.Scale.Y;

            childTransform.Position = new Vector2(
                parentTransform.Position.X + (localX * cos - localY * sin),
                parentTransform.Position.Y + (localX * sin + localY * cos)
            );
            childTransform.Rotation = parentTransform.Rotation + local.LocalRotation;
            childTransform.Scale = parentTransform.Scale * local.LocalScale;
        }
    }

    public void Dispose()
    {
        nodes.Dispose();
    }
}

