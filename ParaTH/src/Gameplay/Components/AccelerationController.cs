using Microsoft.Xna.Framework;

namespace ParaTH;

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

// 16 bytes
public struct AccelerationController
{
    public AccelerationInstruction[] Instructions;  // 8
    public sbyte Index;                             // 1
                                                    // 7 padding
}

