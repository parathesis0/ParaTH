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

// 32 bytes to fit exactly 2 or 4 within cache lines, most unreadable code ever
public struct VelocityInstruction(ushort triggerFrame, Vector2 startVelocity, Vector2 endVelocity,
                                  float relativeAngle, ushort duration, EasingFunction ease)
{
    public EasingFunction Ease = ease;
    public Vector2 StartVelocity = startVelocity;   // only start is nan for lerp to, both nan for delay, if relative, magnitute equals to relativeVelocity's magnitude
    public Vector2 EndVelocity = endVelocity;       // both nan for delay, if lerp to, is the destination for the lerp
    public float RelativeAngle = relativeAngle;     // not nan for relative
    public ushort TriggerFrame = triggerFrame;
    public ushort Duration = duration;

    public float SpeedIncrement
    {
        set => StartVelocity.X = value;
        readonly get => StartVelocity.X;
    }

    public readonly bool IsDelay => float.IsNaN(StartVelocity.X) &&
                                    float.IsNaN(StartVelocity.Y) &&
                                    float.IsNaN(EndVelocity.X) &&
                                    float.IsNaN(EndVelocity.Y);

    public readonly bool IsLerpTo => float.IsNaN(StartVelocity.X) &&
                                     float.IsNaN(StartVelocity.Y) &&
                                     !float.IsNaN(EndVelocity.X) &&
                                     !float.IsNaN(EndVelocity.Y);
    public readonly bool IsRelative => !float.IsNaN(RelativeAngle);
}

public struct BulletController
{
    public CurveInstruction[] CurveInstructions;
    public ushort CurveTick;
    public sbyte CurveIndex;

    public VelocityInstruction[] VelocityInstructions;
    public ushort VelocityTick;
    public byte VelocityIndex;
}
