using Microsoft.Xna.Framework;

using static ParaTH.AnimationAsset;

namespace ParaTH;

// 24 bytes
public struct AnimationRenderer(AnimationAsset animation, Color color, byte layer, StgBlendState blendState)
{
    public AnimationAsset Animation = animation;    // 8
    public Color Color = color;                     // 4
    public int Counter;                             // 4
    public ushort FrameIndex;                       // 2
    public bool IsPlaying;                          // 1
    public bool IsReverse;                          // 1
    public byte Layer = layer;                      // 1
    public StgBlendState BlendState = blendState;   // 1
                                                    // 2 padding
    public readonly Frame CurrentFrame => Animation.Frames.UnsafeAt(FrameIndex);
}

