using Microsoft.Xna.Framework;

namespace ParaTH;

// 16 bytes
public struct CurvyLaser
{
    public UnsafePooledQueue<Vector2> LaserNodes;   // 8
    public int Length;                              // 4
    public float HalfWidth;                         // 4
}
