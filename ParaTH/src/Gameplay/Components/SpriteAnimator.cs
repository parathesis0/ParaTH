namespace ParaTH;

using static AnimationAsset;

// 16 bytes
public struct SpriteAnimator(AnimationAsset animation, bool isActive = true)
{
    public AnimationAsset Animation = animation;    // 8
    public int Counter;                             // 4
    public ushort FrameIndex;                       // 2
    public bool IsActive = isActive;                // 1
    public bool IsReverse;                          // 1
    public readonly Frame CurrentFrame => Animation.Frames.UnsafeAt(FrameIndex);
}

