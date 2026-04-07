namespace ParaTH;

// 4 bytes
public struct Lifetime(short offscreenAliveFrames)
{
    public ushort AliveFrames;                                  // 2
    public short OffscreenFramesToLive = offscreenAliveFrames;  // 2
}
