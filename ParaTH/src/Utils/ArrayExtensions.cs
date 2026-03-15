using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ParaTH;

public static class ArrayExtensions
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
}
