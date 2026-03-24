using Microsoft.Xna.Framework;

namespace ParaTH;

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

// 32 bytes
public struct VelocityController
{
    public VelocityInstruction[] Instructions;  // 8
    public Vector2 StartValue;                  // 8
    public Vector2 EndValue;                    // 8
    public sbyte Index;                         // 1
                                                // 7 padding
}
