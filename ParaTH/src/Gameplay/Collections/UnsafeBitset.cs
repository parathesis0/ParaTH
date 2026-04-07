using System.Runtime.CompilerServices;

namespace ParaTH;

[SkipLocalsInit]
public sealed class UnsafeBitset
{
    private ulong[] bits;

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => bits.Length << 6;
    }

    public UnsafeBitset(int bitsCount)
    {
        bits = new ulong[((bitsCount - 1) >> 6) + 1];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int index)
    {
        bits.UnsafeAt(index >> 6) |= 1UL << (index & 0x3F);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSet(int index)
    {
        return (bits.UnsafeAt(index >> 6) & (1UL << (index & 0x3F))) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unset(int index)
    {
        bits.UnsafeAt(index >> 6) &= ~(1UL << (index & 0x3F));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        bits.AsSpan().Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureCapacity(int minBitsCount)
    {
        if (minBitsCount <= Capacity)
            return;

        Array.Resize(ref bits, ((minBitsCount - 1) >> 6) + 1);
    }

    public void TrimExcess()
    {
        int lastUsed = -1;
        for (int i = bits.Length - 1; i >= 0; i--)
        {
            if (bits.UnsafeAt(i) != 0)
            {
                lastUsed = i;
                break;
            }
        }

        int newLength = Math.Max(lastUsed + 1, 1);
        if (newLength < bits.Length)
            Array.Resize(ref bits, newLength);
    }
}
