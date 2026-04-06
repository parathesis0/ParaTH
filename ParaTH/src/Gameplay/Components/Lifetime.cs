namespace ParaTH;

// 6 bytes
public struct Lifetime(short offscreenAliveFrames)
{
    public ushort AliveFrames;                                  // 2
    public short OffscreenFramesToLive = offscreenAliveFrames;  // 2
    public bool IsReadyToDie = false;                           // 1, this really shouldn't be here
                                                                // 1 padding
}
