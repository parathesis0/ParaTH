using Microsoft.Xna.Framework;

namespace ParaTH;

// 16 bytes
public struct PositionInstruction(ushort triggerFrame, Vector2 end, ushort duration,
                                  EaseType type, PositionInstruction.Ops op)
{
    public enum Ops : byte
    {
        Set,
        Add
    }

    public Vector2 Params = end;                // 8
    public ushort TriggerFrame = triggerFrame;  // 2
    public ushort Duration = duration;          // 2
    public EaseType EaseType = type;            // 1
    public Ops Op = op;                         // 1
                                                // 2 padding
}

// 16 bytes
public struct VelocityInstruction(ushort triggerFrame, Vector2 @params, ushort duration,
                                  EaseType type, VelocityInstruction.Ops op)
{
    public enum Ops : byte
    {
        SetVelocity, SetMagnitude, SetAngle,
        AddVelocity, AddMagnitude, AddAngle
    }

    public Vector2 Params = @params;            // 8
    public ushort TriggerFrame = triggerFrame;  // 2
    public ushort Duration = duration;          // 2
    public EaseType EaseType = type;            // 1
    public Ops Op = op;                         // 1
                                                // 2 padding
}

// 12 bytes
public struct AccelerationInstruction(ushort triggerFrame, Vector2 @params,
                                      AccelerationInstruction.Ops op)
{
    public enum Ops : byte
    {
        SetAcceleration, SetMagnitude, SetAngle,
        AddAcceleration, AddMagnitude, AddAngle
    }

    public Vector2 Params = @params;            // 8
    public ushort TriggerFrame = triggerFrame;  // 2
    public Ops Op = op;                         // 1
                                                // 1 padding
}

// 8 bytes
// todo: impl AddAngleVelocity? Is it even possible without a field tracking the current AV or keep the instruction within 8 bytes?
public struct CurveInstruction(ushort triggerFrame, float angularVelocity)
{
    public float AngularVelocity = angularVelocity; // 4
    public ushort TriggerFrame = triggerFrame;      // 2
                                                    // 2 padding
}

// todo: maybe find a way to make this less bloat? offset black magic?
// we have to put these together for less structual changes during creation & better archetyping.
// all instructions have a maximum limit of 127
// instructions is shared among bullets
// currently at 72 bytes
public struct BulletController
{
    public PositionInstruction[] PositionInstructions;          // 8
    public Vector2 PositionStartValue;                          // 8
    public Vector2 PositionEndValue;                            // 8

    public VelocityInstruction[] VelocityInstructions;          // 8
    public Vector2 VelocityStartValue;                          // 8
    public Vector2 VelocityEndValue;                            // 8

    public AccelerationInstruction[] AccelerationInstructions;  // 8

    public CurveInstruction[] CurveInstructions;                // 8

    public sbyte VelocityIndex;     // 1
    public sbyte AccelerationIndex; // 1
    public sbyte PositionIndex;     // 1
    public sbyte CurveIndex;        // 1

    public ushort CurrentFrame;     // 2 TODO: PUT THIS IN LIFETIME COMPONENT GOD DAMNIT
                                    // 2 padding
}
