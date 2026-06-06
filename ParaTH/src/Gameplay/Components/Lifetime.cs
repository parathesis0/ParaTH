namespace ParaTH;

// 6 bytes
public struct Lifetime(short offscreenAliveFrames, ushort maxAliveFrames = 0)
{
    public ushort AliveFrames;                                  // 2
    public ushort MaxAliveFrames = maxAliveFrames;              // 2, 0 means infinite
    public short OffscreenFramesToLive = offscreenAliveFrames;  // 2, negative means disabled
}
