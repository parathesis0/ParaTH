namespace ParaTH;

public struct CurveMovementFrame(float angularVelocity, int triggerFrame)
{
    public int TriggerFrame = triggerFrame;
    public float AngularVelocity = angularVelocity;
}

public struct CurveMovement
{
    public int CurrentFrame;
    public int CurrentIndex;
    public CurveMovementFrame[] Frames;
}
