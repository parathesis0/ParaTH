namespace ParaTH;

// 8 bytes
public struct CurveInstruction(ushort triggerFrame, float angularVelocity)
{
    public float AngularVelocity = angularVelocity; // 4
    public ushort TriggerFrame = triggerFrame;      // 2
                                                    // 2 padding
}

// 16 bytes
public struct CurveController
{
    public CurveInstruction[] Instructions;         // 8
    public sbyte Index;                             // 1
                                                    // 7
}

