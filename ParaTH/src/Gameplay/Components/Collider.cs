using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ParaTH;

// 16 bytes
// todo: more groups? entity binding? cache AABB for prefiltering?
// colliders can have only one Group but and multiple TargetGroups. TargetGroups cannot contain self.
[StructLayout(LayoutKind.Explicit)]
public struct Collider
{
    public const int MaxGroups = 8;
    [FieldOffset(0)] public ShapeType ShapeType;    // 1
    [FieldOffset(1)] public bool IsActive;          // 1
    [FieldOffset(2)] public byte GroupMask;         // 1
    [FieldOffset(3)] public byte TargetGroupMask;   // 1
    [FieldOffset(4)] public ObbRect ObbRect;        // 4 + 4 + 4
    [FieldOffset(4)] public Circle Circle;          // 4
    [FieldOffset(4)] public Ellipse Ellipse;        // 4 + 4 + 4

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(Collider colliderA, Vector2 posA, Collider colliderB, Vector2 posB)
    {
#pragma warning disable CS8509 // The switch expression does not handle all possible values.
        return colliderA.ShapeType switch
        {
            ShapeType.ObbRect => colliderB.ShapeType switch
            {
                ShapeType.ObbRect => CollisionDetector.Intersects(colliderA.ObbRect, posA, colliderB.ObbRect, posB),
                ShapeType.Circle  => CollisionDetector.Intersects(colliderA.ObbRect, posA, colliderB.Circle, posB),
                ShapeType.Ellipse => CollisionDetector.Intersects(colliderA.ObbRect, posA, colliderB.Ellipse, posB),
            },
            ShapeType.Circle  => colliderB.ShapeType switch
            {
                ShapeType.ObbRect => CollisionDetector.Intersects(colliderA.Circle, posA, colliderB.ObbRect, posB),
                ShapeType.Circle  => CollisionDetector.Intersects(colliderA.Circle, posA, colliderB.Circle, posB),
                ShapeType.Ellipse => CollisionDetector.Intersects(colliderA.Circle, posA, colliderB.Ellipse, posB),
            },
            ShapeType.Ellipse => colliderB.ShapeType switch
            {
                ShapeType.ObbRect => CollisionDetector.Intersects(colliderA.Ellipse, posA, colliderB.ObbRect, posB),
                ShapeType.Circle  => CollisionDetector.Intersects(colliderA.Ellipse, posA, colliderB.Circle, posB),
                ShapeType.Ellipse => CollisionDetector.Intersects(colliderA.Ellipse, posA, colliderB.Ellipse, posB),
            },
        };
#pragma warning restore CS8509 // The switch expression does not handle all possible values.
    }
}
