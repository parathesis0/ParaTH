using Microsoft.Xna.Framework;

namespace ParaTH;

// 16 bytes
// lasers that moves and leaves a trail
public struct CurvyLaser
{
    public UnsafePooledQueue<Vector2> LaserNodes;   // 8
    public int MaxNodes;                            // 4
    public float HalfWidth;                         // 4

    public readonly bool IsSpawning => LaserNodes.Count < MaxNodes;
}
