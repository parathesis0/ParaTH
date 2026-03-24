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

// 32 bytes
public struct PositionController
{
    public PositionInstruction[] Instructions;  // 8
    public Vector2 StartValue;                  // 8
    public Vector2 EndValue;                    // 8
    public sbyte Index;                         // 1
                                                // 7 padding
}
