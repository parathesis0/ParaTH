using System.Runtime.InteropServices;

namespace ParaTH;

// 16 bytes
[StructLayout(LayoutKind.Explicit)]
public struct Collider
{
    [FieldOffset(0)] public ShapeType ShapeType;    // 1
    [FieldOffset(1)] public bool IsActive;          // 1
    [FieldOffset(2)] public byte Group;             // 1
    [FieldOffset(3)] public byte TargetGroup;       // 1
    [FieldOffset(4)] public ObbRect ObbRect;        // 12
    [FieldOffset(4)] public Circle Circle;          // 4
    [FieldOffset(4)] public Ellipse Ellipse;        // 12, max is 12
}

