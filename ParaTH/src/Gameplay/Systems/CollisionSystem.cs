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

    private struct CurvyLaserCollisionNode
    {
        public Vector2 BoundingCenter;
        public float BoundingRadius;
        public byte GroupMask;
        public byte TargetGroupMask;
        public Entity Entity;
        public UnsafePooledQueue<Vector2> LaserNodes;
        public float HalfWidth;
    }

    private readonly UnsafePooledList<CollisionNode>[] groupLists;
    private readonly byte[] groupTargetUnions;
    private readonly UnsafePooledList<CurvyLaserCollisionNode> curvyLaserList;

    public CollisionSystem(World world)
    {
        groupLists = new UnsafePooledList<CollisionNode>[Collider.MaxGroups];
        groupTargetUnions = new byte[Collider.MaxGroups];
        for (int i = 0; i < Collider.MaxGroups; i++)
            groupLists.UnsafeAt(i) = new UnsafePooledList<CollisionNode>(128);

        curvyLaserList = new UnsafePooledList<CurvyLaserCollisionNode>(16);
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
            gl.UnsafeAt(i).Clear();
            gtu.UnsafeAt(i) = 0;
        }
        curvyLaserList.Clear();

        var q = world.GetOrCreateQuery(descriptor);

        // pre pass
        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            // archetype level prefiltering
            bool isCurvyLaser = archetype.Has<CurvyLaser>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<Transform, Collider>(
                    out var transforms, out var colliders);

                if (!isCurvyLaser) // regular collisions, hotpath
                {
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
                else // curvy laser, slow and cold
                {
                    chunk.GetFilledComponentSpan<CurvyLaser>(out var curvyLasers);

                    for (int i = 0; i < chunk.EntityCount; i++)
                    {
                        ref var collider = ref colliders.UnsafeAt(i);
                        if (!collider.IsActive)
                            continue;

                        ref var laser = ref curvyLasers.UnsafeAt(i);

                        laser.LaserNodes.AsSpans(out var nodesFirst, out var nodesSecond);
                        ComputeRitterBoundingCircle(nodesFirst, nodesSecond, laser.HalfWidth,
                            out var center, out var radius);

                        int groupIndex = BitOperations.TrailingZeroCount(collider.GroupMask);
                        gtu.UnsafeAt(groupIndex) |= collider.TargetGroupMask;

                        curvyLaserList.Add(new CurvyLaserCollisionNode
                        {
                            BoundingCenter = center,
                            BoundingRadius = radius,
                            GroupMask = collider.GroupMask,
                            TargetGroupMask = collider.TargetGroupMask,
                            Entity = chunk.Entities.UnsafeAt(i),
                            LaserNodes = laser.LaserNodes,
                            HalfWidth = laser.HalfWidth,
                        });
                    }
                }
            }
        }

        // hot path, cross group collision detection
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
                        ref var nodeB = ref Unsafe.Add(ref baseB, b);
                        float dx = nodeA.Position.X - nodeB.Position.X;
                        float dy = nodeA.Position.Y - nodeB.Position.Y;
                        float radiusSum = nodeA.BoundingRadius + nodeB.BoundingRadius;
                        if (dx * dx + dy * dy > radiusSum * radiusSum)
                            continue;

                        if (Collider.Intersects(nodeA.Collider, nodeA.Position, nodeB.Collider, nodeB.Position))
                        {
                            // todo: trigger entity's callback?
                        }
                    }
                }
            }
        }

        // cold path: CurvyLaser vs regular colliders
        var laserSpan = curvyLaserList.AsSpan();

        for (int l = 0; l < laserSpan.Length; l++)
        {
            ref var laser = ref laserSpan.UnsafeAt(l);

            for (int g = 0; g < Collider.MaxGroups; g++)
            {
                var list = gl.UnsafeAt(g);
                if (list.Count == 0)
                    continue;

                bool laserTargetsG = (laser.TargetGroupMask & (1 << g)) != 0;
                bool gTargetsLaser = (gtu.UnsafeAt(g) & laser.GroupMask) != 0;
                if (!laserTargetsG && !gTargetsLaser)
                    continue;

                var regularSpan = list.AsSpan();
                for (int c = 0; c < regularSpan.Length; c++)
                {
                    ref var node = ref regularSpan.UnsafeAt(c);

                    // bounding circle broad phase
                    float dx = laser.BoundingCenter.X - node.Position.X;
                    float dy = laser.BoundingCenter.Y - node.Position.Y;
                    float rSum = laser.BoundingRadius + node.BoundingRadius;
                    if (dx * dx + dy * dy > rSum * rSum)
                        continue;

                    // per-node narrow phase
                    if (IntersectsLaserVsCollider(ref laser, ref node))
                    {
                        // todo: trigger entity's callback?
                    }
                }
            }
        }

        // really cold and slow path: CurvyLaser vs CurvyLaser
        for (int a = 0; a < laserSpan.Length; a++)
        {
            ref var laserA = ref laserSpan.UnsafeAt(a);

            for (int b = a + 1; b < laserSpan.Length; b++)
            {
                ref var laserB = ref laserSpan.UnsafeAt(b);

                bool aTargetsB = (laserA.TargetGroupMask & laserB.GroupMask) != 0;
                bool bTargetsA = (laserB.TargetGroupMask & laserA.GroupMask) != 0;
                if (!aTargetsB && !bTargetsA)
                    continue;

                float dx = laserA.BoundingCenter.X - laserB.BoundingCenter.X;
                float dy = laserA.BoundingCenter.Y - laserB.BoundingCenter.Y;
                float rSum = laserA.BoundingRadius + laserB.BoundingRadius;
                if (dx * dx + dy * dy > rSum * rSum)
                    continue;

                if (IntersectsLaserVsLaser(ref laserA, ref laserB))
                {
                    // todo: trigger entity's callback?
                }
            }
        }
    }

    // helpers for regular colliders, hot
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetBoundingRadius(ref Collider c)
    {
#pragma warning disable CS8509
        return c.ShapeType switch
        {
            ShapeType.Circle => c.Circle.Radius,
            ShapeType.ObbRect => c.ObbRect.HalfSize.Length(),
            ShapeType.Ellipse => MathF.Max(c.Ellipse.HalfSize.X, c.Ellipse.HalfSize.Y),
        };
#pragma warning restore CS8509
    }

    // helpers for CurvyLaser, fairly cold
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float DistanceSq(Vector2 a, Vector2 b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        return dx * dx + dy * dy;
    }

    // index into the virtual concatenation of two spans.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ref Vector2 AtConcat(Span<Vector2> first, Span<Vector2> second, int i)
    {
        if (i < first.Length)
            return ref first.UnsafeAt(i);
        return ref second.UnsafeAt(i - first.Length);
    }

    // ritter's bounding-circle over two contiguous spans from ring buffer,
    // inflated by halfWidth
    private static void ComputeRitterBoundingCircle(
        Span<Vector2> first, Span<Vector2> second, float halfWidth,
        out Vector2 center, out float radius)
    {
        int totalCount = first.Length + second.Length;

        if (totalCount == 1)
        {
            center = AtConcat(first, second, 0);
            radius = halfWidth;
            return;
        }

        var p0 = AtConcat(first, second, 0);

        // initial diameter, two mutually most-distant points
        int fA = 0;
        float maxSq = 0f;
        for (int i = 1; i < totalCount; i++)
        {
            float sq = DistanceSq(p0, AtConcat(first, second, i));
            if (sq > maxSq) { maxSq = sq; fA = i; }
        }

        var pA = AtConcat(first, second, fA);
        int fB = 0;
        maxSq = 0f;
        for (int i = 0; i < totalCount; i++)
        {
            float sq = DistanceSq(pA, AtConcat(first, second, i));
            if (sq > maxSq) { maxSq = sq; fB = i; }
        }

        center = (AtConcat(first, second, fA) + AtConcat(first, second, fB)) * 0.5f;
        radius = MathF.Sqrt(maxSq) * 0.5f;

        // grow to enclose every point
        for (int i = 0; i < totalCount; i++)
        {
            var pt = AtConcat(first, second, i);
            float dSq = DistanceSq(center, pt);
            if (dSq <= radius * radius)
                continue;

            float d = MathF.Sqrt(dSq);
            float newR = (radius + d) * 0.5f;
            float k = (newR - radius) / d;
            center += (pt - center) * k;
            radius = newR;
        }

        // don't forget to add
        radius += halfWidth;
    }

    // narrow phase: each laser node circle vs shaped collider
    private static bool IntersectsLaserVsCollider(
        ref CurvyLaserCollisionNode laser,
        ref CollisionNode node)
    {
        laser.LaserNodes.AsSpans(out var first, out var second);
        var circle = new Circle(laser.HalfWidth);
        float rSum = laser.HalfWidth + node.BoundingRadius;
        float rSumSq = rSum * rSum;

        // iterate first segment
        for (int i = 0; i < first.Length; i++)
        {
            ref var lp = ref first.UnsafeAt(i);
            float dx = lp.X - node.Position.X;
            float dy = lp.Y - node.Position.Y;
            if (dx * dx + dy * dy > rSumSq)
                continue;
            if (IntersectsCircleVsShape(circle, lp, ref node.Collider, node.Position))
                return true;
        }

        // iterate second segment (wrap-around portion, may be empty)
        for (int i = 0; i < second.Length; i++)
        {
            ref var lp = ref second.UnsafeAt(i);
            float dx = lp.X - node.Position.X;
            float dy = lp.Y - node.Position.Y;
            if (dx * dx + dy * dy > rSumSq)
                continue;
            if (IntersectsCircleVsShape(circle, lp, ref node.Collider, node.Position))
                return true;
        }

        return false;
    }

    private static bool IntersectsCircleVsShape(
        Circle circle, Vector2 circlePos,
        ref Collider collider, Vector2 colliderPos)
    {
#pragma warning disable CS8509
        return collider.ShapeType switch
        {
            ShapeType.ObbRect => CollisionDetector.Intersects(circle, circlePos, collider.ObbRect, colliderPos),
            ShapeType.Circle => CollisionDetector.Intersects(circle, circlePos, collider.Circle, colliderPos),
            ShapeType.Ellipse => CollisionDetector.Intersects(circle, circlePos, collider.Ellipse, colliderPos),
        };
#pragma warning restore CS8509
    }

    // narrow phase: each node circle A vs each node circle B.
    private static bool IntersectsLaserVsLaser(
        ref CurvyLaserCollisionNode laserA,
        ref CurvyLaserCollisionNode laserB)
    {
        laserA.LaserNodes.AsSpans(out var firstA, out var secondA);
        laserB.LaserNodes.AsSpans(out var firstB, out var secondB);
        float rSum = laserA.HalfWidth + laserB.HalfWidth;
        float rSumSq = rSum * rSum;

        // test all 4 combinations:
        // firstA x firstB, firstA x secondB,
        // secondA x firstB, secondA x secondB
        if (TestSpanPairCircles(firstA, firstB, rSumSq)) return true;
        if (secondB.Length > 0 && TestSpanPairCircles(firstA, secondB, rSumSq)) return true;
        if (secondA.Length > 0)
        {
            if (TestSpanPairCircles(secondA, firstB, rSumSq)) return true;
            if (secondB.Length > 0 && TestSpanPairCircles(secondA, secondB, rSumSq)) return true;
        }
        return false;
    }

    private static bool TestSpanPairCircles(
        Span<Vector2> spanA, Span<Vector2> spanB, float rSumSq)
    {
        for (int a = 0; a < spanA.Length; a++)
        {
            ref var pa = ref spanA.UnsafeAt(a);
            for (int b = 0; b < spanB.Length; b++)
            {
                ref var pb = ref spanB.UnsafeAt(b);
                float dx = pa.X - pb.X;
                float dy = pa.Y - pb.Y;
                if (dx * dx + dy * dy <= rSumSq)
                    return true;
            }
        }
        return false;
    }

    public void Dispose()
    {
        foreach (var list in groupLists)
            list.Dispose();
        curvyLaserList.Dispose();
    }
}
