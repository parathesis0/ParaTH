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

// 28 bytes, as small as it can be without reducing vector percision. most unreadable code ever, the shit we do to trim sizes...
// we have four modes here. Set/Adds happen in an instant and Lerps happen over frameDuration
// - Set/Lerp
//     directly override Velocity with a fixed vector.
//     default behaviour. straightfoward.
//     start is StartVelocity, end is EndVelocity.
// - Add/LerpAdd
//     add a vector to the current Velocity.
//     EndVelocity.X == NaN is the flag for this.
//     StartVelocity is the velocity vector to add.
// - Set/LerpRelative
//     directly override Velocity with a vector whose magnitude is fixed angle is relative to the Velocity's angle.
//     EndVelocity.Y == NaN is the flag for this.
//     StartVelocity.X is the new vector's magnitude, StartVelocity.Y is the relative angle.
// - AddRelative/LerpAddRelative
//     add a vector whose magnitude is fixed angle is relative to the Velocity's angle to the current Velocity
//     EndVelocity.X == NaN && EndVelocity.Y == NaN is the flag for this.
//     StartVelocity.X is the added vector's magnitude, StartVelocity.Y is the relative angle.
public struct VelocityInstruction(ushort triggerFrame, Vector2 start, Vector2 end, ushort duration, EasingFunction easing)
{
    public EasingFunction Ease = easing;
    public Vector2 StartVelocity = start;
    public Vector2 EndVelocity = end;
    public ushort TriggerFrame = triggerFrame;
    public ushort Duration = duration;

    public readonly float NewVelocityMagnitude => StartVelocity.X;
    public readonly float AngleDelta => StartVelocity.Y;
    public readonly Vector2 AddEndVelocity => StartVelocity;
    public readonly bool IsDelay => !float.IsNaN(StartVelocity.X) &&
                                    !float.IsNaN(StartVelocity.Y) &&
                                    float.IsNaN(EndVelocity.X) &&
                                    float.IsNaN(EndVelocity.Y);
    public readonly bool IsAdd => float.IsNaN(EndVelocity.X);
    public readonly bool IsRelative => float.IsNaN(EndVelocity.Y);
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
