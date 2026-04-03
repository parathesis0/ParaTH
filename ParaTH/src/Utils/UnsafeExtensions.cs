using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ParaTH;

public static class UnsafeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T UnsafeAt<T>(this T[] array, int index)
    {
#if !DEBUG
        return ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), (nint)(uint)index);
#else
        return ref array[index];
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T UnsafeAt<T>(this Span<T> span, int index)
    {
#if !DEBUG
        return ref Unsafe.Add(ref MemoryMarshal.GetReference(span), (nint)(uint)index);
#else
        return ref span[index];
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T UnsafeAt<T>(this ReadOnlySpan<T> span, int index)
    {
#if !DEBUG
        return ref Unsafe.Add(ref MemoryMarshal.GetReference(span), (nint)(uint)index);
#else
        return ref span[index];
#endif
    }
}
