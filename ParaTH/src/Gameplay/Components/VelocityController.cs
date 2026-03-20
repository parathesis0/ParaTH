using Microsoft.Xna.Framework;

namespace ParaTH;

public struct VelocityInstruction(
    EasingFunction ease, Vector2 startVelocity, Vector2 endVelocity, ushort triggerFrame, ushort duration, bool isRelative)
{
    public EasingFunction Ease = ease;
    public Vector2 StartVelocity = startVelocity;
    public Vector2 EndVelocity = endVelocity;

    public ushort TriggerFrame = triggerFrame;
    public ushort Duration = duration;
    public bool IsRelative = isRelative;
}

public struct VelocityController
{
    public VelocityInstruction[] Instructions;
    public ushort TickCount;
    public byte CurrentIndex;

    public readonly VelocityInstruction CurrentInstrucion => Instructions.UnsafeAt(CurrentIndex);
    public readonly ref VelocityInstruction CurrentInstrucionRef => ref Instructions.UnsafeAt(CurrentIndex);
}
