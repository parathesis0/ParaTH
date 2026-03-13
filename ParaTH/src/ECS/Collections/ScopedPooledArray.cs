using System.Buffers;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace ParaTH;

public readonly record struct ScopedPooledArray<T>(T[] Array, int Length) : IDisposable
{
    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Array.UnsafeAt(index);
    }

    public Span<T> AsSpan()
    {
        return MemoryMarshal.CreateSpan(ref this[0], Length);
    }

    public static ScopedPooledArray<T> Rent(int amount)
    {
        return new ScopedPooledArray<T>(ArrayPool<T>.Shared.Rent(amount), amount);
    }

    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(Array);
    }

    public static implicit operator T[](ScopedPooledArray<T> scoped)
    {
        return scoped.Array;
    }
}
