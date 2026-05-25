namespace ParaTH;

// 12 bytes
public struct RotationInstruction(ushort triggerFrame, float @params, ushort duration,
                                  EaseType type, RotationInstruction.Ops op)
{
    public enum Ops : byte
    {
        SetRotation, SetRotationalVelocity,
        AddRotation, AddRotationalVelocity
    }

    public float Params = @params;              // 4
    public ushort TriggerFrame = triggerFrame;  // 2
    public ushort Duration = duration;          // 2
    public EaseType EaseType = type;            // 1
    public Ops Op = op;                         // 1
                                                // 2 padding
}

// 24 bytes
public struct RotationController
{
    public RotationInstruction[] Instructions;      // 8
    public float StartRotation;                     // 4
    public float EndRotation;                       // 4
    public float RotationalVelocity;                // 4
    public sbyte Index;                             // 1
                                                    // 3 padding
}
