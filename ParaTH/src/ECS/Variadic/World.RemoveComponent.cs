using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed partial class World
{
    // putting this here for now
    [SkipLocalsInit]
    private static T[] Remove<T>(T[] a, T[] b) where T : IEquatable<T>
    {
        var result = GC.AllocateUninitializedArray<T>(a.Length - b.Length);
        int ri = 0;
        ReadOnlySpan<T> bSpan = b;

        for (int i = 0; i < a.Length; i++)
        {
            if (!bSpan.Contains(a.UnsafeAt(i)))
                result.UnsafeAt(ri++) = a.UnsafeAt(i);
        }
        return result;
    }

    [SkipLocalsInit]
    public void RemoveComponent<T0, T1>(Entity entity)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Remove(oldArchetype.ComponentTypes, Component<T0, T1>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        // moving to a archetype with less component types removes the component during chunk's copying
        Move(ref entityData, oldArchetype, newArchetype);
    }
}
