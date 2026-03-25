using Microsoft.Xna.Framework;

using static ParaTH.AnimationAsset;

namespace ParaTH;

// 16 bytes
public struct AnimationRenderer(AnimationAsset animation)
{
    public AnimationAsset Animation = animation;    // 8
    public int Counter;                             // 4
    public ushort FrameIndex;                       // 2
    public bool IsPlaying;                          // 1
    public bool IsReverse;                          // 1
    public readonly Frame CurrentFrame => Animation.Frames.UnsafeAt(FrameIndex);
}

