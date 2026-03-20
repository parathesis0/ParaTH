namespace ParaTH;

public struct CurveMovementInstruction(float angularVelocity, ushort triggerFrame,
                                       sbyte jumpToindex, byte loopTimes)
{
    public float AngularVelocity = angularVelocity;
    public ushort TriggerFrame = triggerFrame;

    public sbyte TargetIndex = jumpToindex; // negative for not jump
    public byte LoopRepeatTimes = loopTimes;

    public const byte Infinite = byte.MaxValue;
}

public struct CurveMovementController
{
    public CurveMovementInstruction[] Instructions;
    public ushort TickCount;
    public sbyte CurrentIndex;
}
