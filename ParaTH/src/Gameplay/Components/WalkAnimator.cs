namespace ParaTH;

using static AnimationAsset;

// 16 bytes
public struct WalkAnimator(
    AnimationAsset idle,
    AnimationAsset left,
    AnimationAsset right,
    AnimationAsset leftTransit,
    AnimationAsset rightTransit)
{
    public enum State : byte
    {
        IdleLoop = 0,
        LeftLoop = 1,
        RightLoop = 2,
        TransitionLeftLoop = 3,
        TransitionRightLoop = 4
    }

    public AnimationAsset[] Animations =
        [idle, left, right, leftTransit, rightTransit]; // 8
    public int Counter;                                 // 4
    public ushort FrameIndex;                           // 2
    private byte packedStateData = 0;                   // 1
    public bool IsActive = true;                        // 1

    public bool IsReverse
    {
        readonly get => (packedStateData & 0b0000_0001) != 0;
        set => packedStateData = (byte)(value ? (packedStateData | 0b0000_0001) : (packedStateData & ~0b0000_0001));
    }

    public State CurrentState
    {
        readonly get => (State)((packedStateData >> 1) & 0b0000_0111);
        set => packedStateData = (byte)((packedStateData & 0b1111_0001) | (((byte)value & 0b0000_0111) << 1));
    }

    public sbyte CurrentDirection
    {
        readonly get
        {
            int val = (packedStateData >> 4) & 0b0000_0011;
            return val == 2 ? (sbyte)-1 : (sbyte)val;
        }
        set
        {
            int val = value == -1 ? 2 : value;
            packedStateData = (byte)((packedStateData & 0b1100_1111) | ((val & 0b0000_0011) << 4));
        }
    }

    public readonly AnimationAsset CurrentAnimation => Animations[(int)CurrentState];
    public readonly Frame CurrentFrame => CurrentAnimation.Frames.UnsafeAt(FrameIndex);
}
