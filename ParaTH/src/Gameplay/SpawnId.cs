using System.Runtime.CompilerServices;

namespace ParaTH;

public static class SpawnId
{
    private static uint next;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Next()
    {
        return next++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint NextBlock(uint count)
    {
        uint start = next;
        next += count;
        return start;
    }
}
