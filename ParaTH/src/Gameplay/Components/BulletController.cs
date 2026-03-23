using Microsoft.Xna.Framework;

namespace ParaTH;

// 24 bytes
public struct PositionInstruction(ushort triggerFrame, Vector2 end, ushort duration,
                                  EasingFunction easing, PositionInstruction.Ops op)
{
    public enum Ops : byte
    {
        Delay,
        Set,
        Add
    }

    public EasingFunction Ease = easing;
    public Vector2 Params = end;
    public ushort TriggerFrame = triggerFrame;
    public ushort Duration = duration;
    public Ops Op = op;
}

// 24 bytes
public struct VelocityInstruction(ushort triggerFrame, Vector2 @params, ushort duration,
                                  EasingFunction easing, VelocityInstruction.Ops op)
{
    public enum Ops : byte
    {
        Delay,
        SetVelocity, SetMagnitude, SetAngle,
        AddVelocity, AddMagnitude, AddAngle
    }

    public EasingFunction Ease = easing;
    public Vector2 Params = @params;
    public ushort TriggerFrame = triggerFrame;
    public ushort Duration = duration;
    public Ops Op = op;
}

// 24 bytes
public struct AccelerationInstruction(ushort triggerFrame, Vector2 @params, ushort duration,
                                      EasingFunction easing, AccelerationInstruction.Ops op)
{
    public enum Ops : byte
    {
        Delay,
        SetAcceleration, SetMagnitude, SetAngle,
        AddAcceleration, AddMagnitude, AddAngle
    }

    public EasingFunction Ease = easing;
    public Vector2 Params = @params;
    public ushort TriggerFrame = triggerFrame;
    public ushort Duration = duration;
    public Ops Op = op;
}

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

// todo: find a way to make this less bloat, remove curve looping so that we can unify the Ticks & Indexes?
// all instructions have a maximum number of 127
// lerp start value & end value is kept
// instructions is shared among bullets
public struct BulletController
{
    public PositionInstruction[] PositionInstructions;
    public ushort PositionTick;
    public sbyte PositionIndex;
    public Vector2 PositionStartValue;
    public Vector2 PositionEndValue;

    public VelocityInstruction[] VelocityInstructions;
    public ushort VelocityTick;
    public sbyte VelocityIndex;
    public Vector2 VelocityStartValue;
    public Vector2 VelocityEndValue;

    public AccelerationInstruction[] AccelerationInstructions;
    public ushort AccelerationTick;
    public sbyte AccelerationIndex;
    public Vector2 AccelerationStartValue;
    public Vector2 AccelerationEndValue;

    public CurveInstruction[] CurveInstructions;
    public ushort CurveTick;
    public sbyte CurveIndex;
}
