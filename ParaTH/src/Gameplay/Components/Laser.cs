using Microsoft.Xna.Framework;

namespace ParaTH;

// 16 bytes
// static lasers
public struct Laser
{
    public UnsafePooledList<Vector2> LaserNodes;    // 8
    public float HalfWidth;                         // 4
                                                    // 4 padding
}
