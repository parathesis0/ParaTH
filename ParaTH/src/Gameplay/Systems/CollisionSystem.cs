using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ParaTH;

// handles cross group collisions between entities with Colliders and CurvyLasers
[SkipLocalsInit]
public sealed class CollisionSystem : IDisposable
{
    private readonly World world;

    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, Collider>();

    private struct CollisionNode
    {
        public Vector2 Position;
        public float Rotation;
        public float BoundingRadius;
        public Collider Collider;
        public Entity Entity;
    }

    private struct CurvyLaserCollisionNode
    {
        public UnsafePooledQueue<Vector2> LaserNodes;
        public Vector2 BoundingCenter;
        public float BoundingRadius;
        public Entity Entity;
        public float HalfWidth;
        public byte GroupMask;
        public byte TargetGroupMask;
    }

    private struct LaserCollisionNode
    {
        public int NodeStartIndex;          // offset into laserRotatedNodes
        public int NodeCount;
        public Vector2 BoundingCenter;
        public float BoundingRadius;
        public Entity Entity;
        public float HalfWidth;
        public byte GroupMask;
        public byte TargetGroupMask;
    }

    private readonly UnsafePooledList<CollisionNode>[] groupLists;
    private readonly byte[] groupTargetUnions;
    private readonly UnsafePooledList<CurvyLaserCollisionNode> curvyLaserList;
    private readonly UnsafePooledList<LaserCollisionNode> laserList;
    private readonly UnsafePooledList<Vector2> laserRotatedNodes;

    public CollisionSystem(World world)
    {
        groupLists = new UnsafePooledList<CollisionNode>[Collider.MaxGroups];
        groupTargetUnions = new byte[Collider.MaxGroups];
        for (int i = 0; i < Collider.MaxGroups; i++)
            groupLists.UnsafeAt(i) = new UnsafePooledList<CollisionNode>(128);

        curvyLaserList = new UnsafePooledList<CurvyLaserCollisionNode>(16);
        laserList = new UnsafePooledList<LaserCollisionNode>(16);
        laserRotatedNodes = new UnsafePooledList<Vector2>(64);
        this.world = world;
    }

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
        laserList.Clear();
        laserRotatedNodes.Clear();

        var q = world.GetOrCreateQuery(descriptor);

        // pre pass
        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            // archetype level prefiltering
            bool isCurvyLaser = archetype.Has<CurvyLaser>();
            bool isLaser = !isCurvyLaser && archetype.Has<Laser>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<Transform, Collider>(
                    out var transforms, out var colliders);

                if (!isCurvyLaser && !isLaser) // regular collisions, hotpath
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
                            Rotation = transforms.UnsafeAt(i).Rotation,
                            Collider = collider,
                            BoundingRadius = GetBoundingRadius(ref collider)
                        });

                        gtu.UnsafeAt(groupIndex) |= collider.TargetGroupMask;
                    }
                }
                else if (isCurvyLaser) // curvy laser, slow and cold
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
                else // laser, slow and cold
                {
                    chunk.GetFilledComponentSpan<Laser>(out var lasers);

                    for (int i = 0; i < chunk.EntityCount; i++)
                    {
                        ref var collider = ref colliders.UnsafeAt(i);
                        if (!collider.IsActive)
                            continue;

                        ref var laser = ref lasers.UnsafeAt(i);
                        ref var transform = ref transforms.UnsafeAt(i);

                        int nodeCount = laser.LaserNodes.Count;
                        if (nodeCount == 0)
                            continue;

                        int startIdx = laserRotatedNodes.Count;
                        AppendRotatedLaserNodes(laser.LaserNodes, transform.Rotation);

                        // bounding circle via Ritter on the just-appended slice.
                        // Re-fetch span here (may have been resized by AppendRotatedLaserNodes).
                        var rotatedSpan = laserRotatedNodes.AsSpan().Slice(startIdx, nodeCount);
                        ComputeLaserBoundingCircle(rotatedSpan, laser.HalfWidth,
                            out var center, out var radius);

                        int groupIndex = BitOperations.TrailingZeroCount(collider.GroupMask);
                        gtu.UnsafeAt(groupIndex) |= collider.TargetGroupMask;

                        laserList.Add(new LaserCollisionNode
                        {
                            NodeStartIndex = startIdx,
                            NodeCount = nodeCount,
                            BoundingCenter = center,
                            BoundingRadius = radius,
                            GroupMask = collider.GroupMask,
                            TargetGroupMask = collider.TargetGroupMask,
                            Entity = chunk.Entities.UnsafeAt(i),
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

                        if (Collider.Intersects(nodeA.Collider, nodeA.Position, nodeA.Rotation,
                                                nodeB.Collider, nodeB.Position, nodeB.Rotation))
                        {
                            // todo: trigger entity's callback?
                            //Console.WriteLine($"Entity {nodeA.Entity.Id} hit Entity {nodeB.Entity.Id}!");
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
                        //Console.WriteLine($"Laser {laser.Entity.Id} hit Entity {node.Entity.Id}!");
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
                    //Console.WriteLine($"Laser {laserA.Entity.Id} hit Laser {laserB.Entity.Id}!");
                }
            }
        }

        // cold path: Laser vs regular colliders
        var staticLaserSpan = laserList.AsSpan();
        var rotatedNodesSpan = laserRotatedNodes.AsSpan();

        for (int l = 0; l < staticLaserSpan.Length; l++)
        {
            ref var laser = ref staticLaserSpan.UnsafeAt(l);

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

                    // per-segment narrow phase
                    if (IntersectsStaticLaserVsCollider(ref laser, rotatedNodesSpan, ref node))
                    {
                        // todo: trigger entity's callback?
                        //Console.WriteLine($"Laser {laser.Entity.Id} hit Entity {node.Entity.Id}!");
                    }
                }
            }
        }

        // really cold path: Laser vs Laser
        for (int a = 0; a < staticLaserSpan.Length; a++)
        {
            ref var laserA = ref staticLaserSpan.UnsafeAt(a);

            for (int b = a + 1; b < staticLaserSpan.Length; b++)
            {
                ref var laserB = ref staticLaserSpan.UnsafeAt(b);

                bool aTargetsB = (laserA.TargetGroupMask & laserB.GroupMask) != 0;
                bool bTargetsA = (laserB.TargetGroupMask & laserA.GroupMask) != 0;
                if (!aTargetsB && !bTargetsA)
                    continue;

                float dx = laserA.BoundingCenter.X - laserB.BoundingCenter.X;
                float dy = laserA.BoundingCenter.Y - laserB.BoundingCenter.Y;
                float rSum = laserA.BoundingRadius + laserB.BoundingRadius;
                if (dx * dx + dy * dy > rSum * rSum)
                    continue;

                if (IntersectsStaticLaserVsStaticLaser(ref laserA, ref laserB, rotatedNodesSpan))
                {
                    // todo: trigger entity's callback?
                    //Console.WriteLine($"Laser {laserA.Entity.Id} hit Laser {laserB.Entity.Id}!");
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

    // todo: this is a little too expensive for a prefilter
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
            var lp = first.UnsafeAt(i);
            float dx = lp.X - node.Position.X;
            float dy = lp.Y - node.Position.Y;
            if (dx * dx + dy * dy > rSumSq)
                continue;
            if (IntersectsCircleVsShape(circle, lp, ref node.Collider, node.Position, node.Rotation))
                return true;
        }

        // iterate second segment (wrap-around portion, may be empty)
        for (int i = 0; i < second.Length; i++)
        {
            var lp = first.UnsafeAt(i);
            float dx = lp.X - node.Position.X;
            float dy = lp.Y - node.Position.Y;
            if (dx * dx + dy * dy > rSumSq)
                continue;
            if (IntersectsCircleVsShape(circle, lp, ref node.Collider, node.Position, node.Rotation))
                return true;
        }

        return false;
    }

    private static bool IntersectsCircleVsShape(
        Circle circle, Vector2 circlePos,
        ref Collider collider, Vector2 colliderPos, float colliderRot)
    {
#pragma warning disable CS8509
        return collider.ShapeType switch
        {
            ShapeType.ObbRect => CollisionDetector.Intersects(circle, circlePos, collider.ObbRect, colliderPos, colliderRot),
            ShapeType.Circle => CollisionDetector.Intersects(circle, circlePos, collider.Circle, colliderPos),
            ShapeType.Ellipse => CollisionDetector.Intersects(circle, circlePos, collider.Ellipse, colliderPos, colliderRot),
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
        if (TestSpanPairCircles(firstA, firstB, rSumSq))
            return true;
        if (secondB.Length > 0 && TestSpanPairCircles(firstA, secondB, rSumSq))
            return true;
        if (secondA.Length > 0)
        {
            if (TestSpanPairCircles(secondA, firstB, rSumSq))
                return true;
            if (secondB.Length > 0 && TestSpanPairCircles(secondA, secondB, rSumSq))
                return true;
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

    // helpers for static Laser, cold

    // append laser nodes rotated around LaserNodes[0] by rotation into the shared buffer
    private void AppendRotatedLaserNodes(UnsafePooledList<Vector2> raw, float rotation)
    {
        var src = raw.AsSpan();
        int n = src.Length;
        if (n == 0) return;

        Vector2 origin = src.UnsafeAt(0);
        laserRotatedNodes.Add(origin);

        if (n == 1) return;

        float cos = MathF.Cos(rotation);
        float sin = MathF.Sin(rotation);

        for (int i = 1; i < n; i++)
        {
            Vector2 rel = src.UnsafeAt(i) - origin;
            laserRotatedNodes.Add(new Vector2(
                origin.X + rel.X * cos - rel.Y * sin,
                origin.Y + rel.X * sin + rel.Y * cos));
        }
    }

    // ritter's bounding circle over a single contiguous span, inflated by halfWidth
    private static void ComputeLaserBoundingCircle(
        Span<Vector2> nodes, float halfWidth,
        out Vector2 center, out float radius)
    {
        int n = nodes.Length;
        if (n == 1)
        {
            center = nodes.UnsafeAt(0);
            radius = halfWidth;
            return;
        }

        var p0 = nodes.UnsafeAt(0);

        int fA = 0;
        float maxSq = 0f;
        for (int i = 1; i < n; i++)
        {
            float sq = DistanceSq(p0, nodes.UnsafeAt(i));
            if (sq > maxSq) { maxSq = sq; fA = i; }
        }

        var pA = nodes.UnsafeAt(fA);
        int fB = 0;
        maxSq = 0f;
        for (int i = 0; i < n; i++)
        {
            float sq = DistanceSq(pA, nodes.UnsafeAt(i));
            if (sq > maxSq) { maxSq = sq; fB = i; }
        }

        center = (nodes.UnsafeAt(fA) + nodes.UnsafeAt(fB)) * 0.5f;
        radius = MathF.Sqrt(maxSq) * 0.5f;

        for (int i = 0; i < n; i++)
        {
            var pt = nodes.UnsafeAt(i);
            float dSq = DistanceSq(center, pt);
            if (dSq <= radius * radius)
                continue;

            float d = MathF.Sqrt(dSq);
            float newR = (radius + d) * 0.5f;
            float k = (newR - radius) / d;
            center += (pt - center) * k;
            radius = newR;
        }

        radius += halfWidth;
    }

    // narrow phase: each laser node (Circle) + each segment (ObbRect) vs shaped collider
    private static bool IntersectsStaticLaserVsCollider(
        ref LaserCollisionNode laser,
        Span<Vector2> rotatedBuffer,
        ref CollisionNode node)
    {
        var nodes = rotatedBuffer.Slice(laser.NodeStartIndex, laser.NodeCount);

        var circle = new Circle(laser.HalfWidth);
        float rSumCircle = laser.HalfWidth + node.BoundingRadius;
        float rSumCircleSq = rSumCircle * rSumCircle;

        // circles at each node
        for (int i = 0; i < nodes.Length; i++)
        {
            var lp = nodes.UnsafeAt(i);
            float dx = lp.X - node.Position.X;
            float dy = lp.Y - node.Position.Y;
            if (dx * dx + dy * dy > rSumCircleSq)
                continue;
            if (IntersectsCircleVsShape(circle, lp, ref node.Collider, node.Position, node.Rotation))
                return true;
        }

        // obb rects between consecutive nodes
        for (int i = 0; i < nodes.Length - 1; i++)
        {
            var a = nodes.UnsafeAt(i);
            var b = nodes.UnsafeAt(i + 1);
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            float lenSq = dx * dx + dy * dy;
            if (lenSq < 1e-6f)
                continue;

            float len = MathF.Sqrt(lenSq);
            float halfLen = len * 0.5f;
            Vector2 center = new(a.X + dx * 0.5f, a.Y + dy * 0.5f);

            // per-segment broad phase
            float segRadius = halfLen + laser.HalfWidth;
            float rSum = segRadius + node.BoundingRadius;
            float ccx = center.X - node.Position.X;
            float ccy = center.Y - node.Position.Y;
            if (ccx * ccx + ccy * ccy > rSum * rSum)
                continue;

            float rot = MathF.Atan2(dy, dx);
            var obb = new ObbRect(new Vector2(halfLen, laser.HalfWidth));

            if (IntersectsObbVsShape(obb, center, rot, ref node.Collider, node.Position, node.Rotation))
                return true;
        }

        return false;
    }

    private static bool IntersectsObbVsShape(
        ObbRect obb, Vector2 obbPos, float obbRot,
        ref Collider collider, Vector2 colliderPos, float colliderRot)
    {
#pragma warning disable CS8509
        return collider.ShapeType switch
        {
            ShapeType.ObbRect => CollisionDetector.Intersects(obb, obbPos, obbRot, collider.ObbRect, colliderPos, colliderRot),
            ShapeType.Circle => CollisionDetector.Intersects(obb, obbPos, obbRot, collider.Circle, colliderPos),
            ShapeType.Ellipse => CollisionDetector.Intersects(obb, obbPos, obbRot, collider.Ellipse, colliderPos, colliderRot),
        };
#pragma warning restore CS8509
    }

    // narrow phase: test each shape of laser A (circles + obb segments) against each shape of laser B
    private static bool IntersectsStaticLaserVsStaticLaser(
        ref LaserCollisionNode laserA,
        ref LaserCollisionNode laserB,
        Span<Vector2> rotatedBuffer)
    {
        var nodesA = rotatedBuffer.Slice(laserA.NodeStartIndex, laserA.NodeCount);
        var nodesB = rotatedBuffer.Slice(laserB.NodeStartIndex, laserB.NodeCount);

        // circles A vs circles B
        float rSum = laserA.HalfWidth + laserB.HalfWidth;
        if (TestSpanPairCircles(nodesA, nodesB, rSum * rSum))
            return true;

        // circles A vs segments B
        var circleA = new Circle(laserA.HalfWidth);
        for (int j = 0; j < nodesB.Length - 1; j++)
        {
            var b0 = nodesB.UnsafeAt(j);
            var b1 = nodesB.UnsafeAt(j + 1);
            if (!BuildSegmentObb(b0, b1, laserB.HalfWidth,
                                 out var segB, out var segBCenter, out var segBRot,
                                 out float segBRadius))
                continue;

            for (int i = 0; i < nodesA.Length; i++)
            {
                var ap = nodesA.UnsafeAt(i);
                float dx = ap.X - segBCenter.X;
                float dy = ap.Y - segBCenter.Y;
                float rs = segBRadius + laserA.HalfWidth;
                if (dx * dx + dy * dy > rs * rs)
                    continue;

                if (CollisionDetector.Intersects(circleA, ap, segB, segBCenter, segBRot))
                    return true;
            }
        }

        // segments A vs circles B
        var circleB = new Circle(laserB.HalfWidth);
        for (int i = 0; i < nodesA.Length - 1; i++)
        {
            var a0 = nodesA.UnsafeAt(i);
            var a1 = nodesA.UnsafeAt(i + 1);
            if (!BuildSegmentObb(a0, a1, laserA.HalfWidth,
                                 out var segA, out var segACenter, out var segARot,
                                 out float segARadius))
                continue;

            for (int j = 0; j < nodesB.Length; j++)
            {
                var bp = nodesB.UnsafeAt(j);
                float dx = bp.X - segACenter.X;
                float dy = bp.Y - segACenter.Y;
                float rs = segARadius + laserB.HalfWidth;
                if (dx * dx + dy * dy > rs * rs)
                    continue;

                if (CollisionDetector.Intersects(circleB, bp, segA, segACenter, segARot))
                    return true;
            }

            // segments A vs segments B
            for (int j = 0; j < nodesB.Length - 1; j++)
            {
                var b0 = nodesB.UnsafeAt(j);
                var b1 = nodesB.UnsafeAt(j + 1);
                if (!BuildSegmentObb(b0, b1, laserB.HalfWidth,
                                     out var segB, out var segBCenter, out var segBRot,
                                     out float segBRadius))
                    continue;

                float dx = segACenter.X - segBCenter.X;
                float dy = segACenter.Y - segBCenter.Y;
                float rs = segARadius + segBRadius;
                if (dx * dx + dy * dy > rs * rs)
                    continue;

                if (CollisionDetector.Intersects(segA, segACenter, segARot, segB, segBCenter, segBRot))
                    return true;
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool BuildSegmentObb(
        Vector2 a, Vector2 b, float halfWidth,
        out ObbRect obb, out Vector2 center, out float rotation, out float boundingRadius)
    {
        float dx = b.X - a.X;
        float dy = b.Y - a.Y;
        float lenSq = dx * dx + dy * dy;
        if (lenSq < 1e-6f)
        {
            obb = default;
            center = default;
            rotation = 0;
            boundingRadius = 0;
            return false;
        }

        float len = MathF.Sqrt(lenSq);
        float halfLen = len * 0.5f;
        center = new Vector2(a.X + dx * 0.5f, a.Y + dy * 0.5f);
        rotation = MathF.Atan2(dy, dx);
        obb = new ObbRect(new Vector2(halfLen, halfWidth));
        boundingRadius = halfLen + halfWidth;
        return true;
    }

    public void Dispose()
    {
        foreach (var list in groupLists)
            list.Dispose();
        curvyLaserList.Dispose();
        laserList.Dispose();
        laserRotatedNodes.Dispose();
    }
}
