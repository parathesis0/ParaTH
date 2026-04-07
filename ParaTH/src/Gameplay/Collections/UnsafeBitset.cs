using System.Buffers;
using System.Runtime.CompilerServices;

namespace ParaTH;

[SkipLocalsInit]
public sealed class UnsafeBitset : IDisposable
{
    private ulong[] bits;
    private int length;

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => length << 6;
    }

    public UnsafeBitset(int bitsCount)
    {
        length = ((bitsCount - 1) >> 6) + 1;
        bits = ArrayPool<ulong>.Shared.Rent(length);
        bits.AsSpan(0, length).Clear();
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
        bits.AsSpan(0, length).Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureCapacity(int minBitsCount)
    {
        int requiredLength = ((minBitsCount - 1) >> 6) + 1;
        if (requiredLength <= length)
            return;

        var newBits = ArrayPool<ulong>.Shared.Rent(requiredLength);
        bits.AsSpan(0, length).CopyTo(newBits);
        newBits.AsSpan(length, requiredLength - length).Clear();

        ArrayPool<ulong>.Shared.Return(bits);
        bits = newBits;
        length = requiredLength;
    }

    public void Dispose()
    {
        ArrayPool<ulong>.Shared.Return(bits);
        bits = null!;
    }
}
