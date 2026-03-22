using Microsoft.Xna.Framework;

namespace ParaTH;

// 32 bytes, 2 can fit within a cache line
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
    public Vector2 StartValue;
    public Vector2 EndValue = end;
    public ushort TriggerFrame = triggerFrame;
    public ushort Duration = duration;
    public Ops Op = op;
}

// 32 bytes, 2 can fit within a cache line
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
    public Vector2 ParamsOrStartValue = @params;
    public Vector2 EndValue; // generated during instruction initialization
    public ushort TriggerFrame = triggerFrame;
    public ushort Duration = duration;
    public Ops Op = op;
}

// 32 bytes, 2 can fit within a cache line
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
    public Vector2 ParamsOrStartValue = @params;
    public Vector2 EndValue; // generated during instruction initialization
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

// todo: find a way to make this less bloat
// all instructions have a maximum number of 127
public struct BulletController
{
    public PositionInstruction[] PositionInstructions;
    public ushort PositionTick;
    public sbyte PositionIndex;

    public VelocityInstruction[] VelocityInstructions;
    public ushort VelocityTick;
    public sbyte VelocityIndex;

    public AccelerationInstruction[] AccelerationInstructions;
    public ushort AccelerationTick;
    public sbyte AccelerationIndex;

    public CurveInstruction[] CurveInstructions;
    public ushort CurveTick;
    public sbyte CurveIndex;
}
