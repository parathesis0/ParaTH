namespace ParaTH;

public struct Lifetime(short offscreenAliveFrames)
{
    public ushort AliveFrames;
    public short OffscreenFramesToLive = offscreenAliveFrames;
}

