using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParaTH;

public sealed partial class World
{
    // putting this here for now
    [SkipLocalsInit]
    static T[] Merge<T>(T[] a, T[] b)
    {
        if (a.Length == 0) return b;
        if (b.Length == 0) return a;

        var result = GC.AllocateUninitializedArray<T>(a.Length + b.Length);
        a.AsSpan().CopyTo(result);
        b.AsSpan().CopyTo(result.AsSpan(a.Length));
        return result;
    }

    [SkipLocalsInit]
    public void AddComponent<T0, T1>(Entity entity, in T0 c0, in T1 c1)
    {
        ref var entityData = ref entityDatas.GetEntityData(entity.Id);
        var oldArchetype = entityData.Archetype;

        // add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to do this
        var newArchetypeTypes = Merge(oldArchetype.ComponentTypes, Component<T0, T1>.GroupTypeInfo);
        var newArchetype = GetOrCreateArchetype(newArchetypeTypes);

        Move(ref entityData, oldArchetype, newArchetype);

        SetComponentValue<T0, T1>(entity, c0, c1);
    }
}
