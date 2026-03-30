using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ParaTH;

public sealed class CollisionSystem : IDisposable
{
    private readonly World world;

    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Collider>();

    private struct CollisionNode
    {
        public Vector2 Position;
        public float BoundingRadius;
        public Collider Collider;
        public Entity Entity;
    }

    private readonly UnsafePooledList<CollisionNode>[] groupLists;
    private readonly byte[] groupTargetUnions;

    public CollisionSystem(World world)
    {
        groupLists = new UnsafePooledList<CollisionNode>[Collider.MaxGroups];
        groupTargetUnions = new byte[Collider.MaxGroups];
        for (int i = 0; i < Collider.MaxGroups; i++)
            groupLists.UnsafeAt(i) = new UnsafePooledList<CollisionNode>(128);

        this.world = world;
    }

    [SkipLocalsInit]
    public void Update()
    {
        var gl = groupLists;
        var gtu = groupTargetUnions;

        // clean up
        for (int i = 0; i < Collider.MaxGroups; i++)
        {
            gl.UnsafeAt(i).Clear(false);
            gtu.UnsafeAt(i) = 0;
        }

        var q = world.GetOrCreateQuery(descriptor);

        // pre pass
        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<Transform, Collider>(
                    out var transforms, out var colliders);

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    ref var collider = ref colliders.UnsafeAt(i);
                    if (!collider.IsActive)
                        continue;

                    int groupIndex = BitOperations.TrailingZeroCount(collider.GroupMask);

                    gl.UnsafeAt(groupIndex).Add(new CollisionNode
                    {
                        Entity = chunk.Entities.UnsafeAt(i),
                        Position = transforms.UnsafeAt(i).Position,
                        Collider = collider,
                        BoundingRadius = GetBoundingRadius(ref collider)
                    });

                    gtu.UnsafeAt(groupIndex) |= collider.TargetGroupMask;
                }
            }
        }

        // cross group collision detection
        for (int i = 0; i < Collider.MaxGroups; i++)
        {
            var listA = gl.UnsafeAt(i);
            if (listA.Count == 0)
                continue;

            byte unionI = gtu.UnsafeAt(i);

            for (int j = i + 1; j < Collider.MaxGroups; j++)
            {
                var listB = gl.UnsafeAt(j);
                if (listB.Count == 0)
                    continue;

                bool iTargetsJ = (unionI & (1 << j)) != 0;
                bool jTargetsI = (gtu.UnsafeAt(j) & (1 << i)) != 0;
                if (!iTargetsJ && !jTargetsI)
                    continue;

                var spanA = listA.AsSpan();
                var spanB = listB.AsSpan();
                ref var baseA = ref MemoryMarshal.GetReference(spanA);
                ref var baseB = ref MemoryMarshal.GetReference(spanB);
                nint countA = spanA.Length;
                nint countB = spanB.Length;

                for (nint a = 0; a < countA; a++)
                {
                    ref var nodeA = ref Unsafe.Add(ref baseA, a);
                    for (nint b = 0; b < countB; b++)
                    {
                        // bounding radius check for quick removal
                        ref var nodeB = ref Unsafe.Add(ref baseB, b);
                        float dx = nodeA.Position.X - nodeB.Position.X;
                        float dy = nodeA.Position.Y - nodeB.Position.Y;
                        float radiusSum = nodeA.BoundingRadius + nodeB.BoundingRadius;
                        if (dx * dx + dy * dy > radiusSum * radiusSum)
                            continue;

                        // accurate collision check
                        if (Collider.Intersects(nodeA.Collider, nodeA.Position, nodeB.Collider, nodeB.Position))
                        {
                            // todo: trigger entity's callback?
                        }
                    }
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetBoundingRadius(ref Collider c)
    {
#pragma warning disable CS8509 // The switch expression does not handle all possible values.
        return c.ShapeType switch
        {
            ShapeType.Circle => c.Circle.Radius,
            ShapeType.ObbRect => c.ObbRect.HalfSize.Length(),
            ShapeType.Ellipse => MathF.Max(c.Ellipse.HalfSize.X, c.Ellipse.HalfSize.Y),
        };
#pragma warning restore CS8509 // The switch expression does not handle all possible values.
    }

    public void Dispose()
    {
        foreach (var list in groupLists)
            list.Dispose();
    }
}
