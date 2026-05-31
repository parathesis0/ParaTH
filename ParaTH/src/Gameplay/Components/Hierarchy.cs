using Microsoft.Xna.Framework;

namespace ParaTH;

// 48 bytes
public struct Hierarchy(
    Entity parent, Vector2 localPosition, Vector2 localScale, float localRotation, bool preserveTransformRotation = false)
{
    public Vector2 LocalPosition = localPosition;                       // 4 + 4
    public Vector2 LocalScale = localScale;                             // 4 + 4
    public float LocalRotation = localRotation;                         // 4
    public Entity Parent = parent;                                      // 4
    public Entity FirstChild;                                           // 4
    public Entity NextSibling;                                          // 4
    public Entity PrevSibling;                                          // 4
    public int Depth = 0;                                               // 4
    public int ChildCount;                                              // 4
    public bool PreserveTransformRotation = preserveTransformRotation;  // 1
                                                                        // 3 padding
}

