using Microsoft.Xna.Framework;

namespace ParaTH;

// 8 bytes
public struct CurveInstruction(ushort triggerFrame, float angularVelocity,
                               sbyte jumpToindex, byte loopTimes)
{
    public float AngularVelocity = angularVelocity;
    public ushort TriggerFrame = triggerFrame;

    public sbyte TargetIndex = jumpToindex; // negative for not jump
    public byte LoopRepeatTimes = loopTimes;

    public const byte Infinite = byte.MaxValue;
}

// 32 bytes, 2 can fit within a cache line
public struct VelocityInstruction(ushort triggerFrame, Vector2 @params, ushort duration, EasingFunction easing, VelocityInstruction.Ops op)
{
    public enum Ops : byte
    {
        Delay,
        SetVelocity, SetMagnitude, SetAngle,
        AddVelocity, AddMagnitude, AddAngle
    }

    public EasingFunction Ease = easing;
    public Vector2 ParamsOrStartValue = @params;
    public Vector2 EndValue; // generated during instruction initialization
    public ushort TriggerFrame = triggerFrame;
    public ushort Duration = duration;
    public Ops Op = op;
}

public struct BulletController
{
    public CurveInstruction[] CurveInstructions;
    public ushort CurveTick;
    public sbyte CurveIndex;    // max 127 instructions

    public VelocityInstruction[] VelocityInstructions;
    public ushort VelocityTick;
    public sbyte VelocityIndex; // max 127 instructions
}
