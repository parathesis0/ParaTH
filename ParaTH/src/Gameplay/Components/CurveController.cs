namespace ParaTH;

// 8 bytes
public struct CurveInstruction(ushort triggerFrame, float angularVelocity, CurveInstruction.Ops op)
{
    public enum Ops : byte
    {
        Set,
        Add
    }

    public float AngularVelocity = angularVelocity; // 4
    public ushort TriggerFrame = triggerFrame;      // 2
    public Ops Op = op;                             // 1
                                                    // 1 padding
}

// 16 bytes
public struct CurveController
{
    public CurveInstruction[] Instructions; // 8
    public float CurrentAngularVelocity;    // 4
    public sbyte Index;                     // 1
                                            // 3 padding
}

