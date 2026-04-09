using System.Diagnostics;

namespace ParaTH;

// add/remove edge lookup supports only one component, for adding/removing 2 or more we'll have to manually merge/remove
public sealed partial class World
{
#pragma warning disable RCS1242 // Do not pass non-read-only struct by read-only reference
    public void QueryAddComponent<T0, T1>(in QueryDescriptor descriptor, in T0 c0, in T1 c1)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1>(setRangeStartSlot, setRangeEndSlot, c0, c1);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2, T3>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2, T3>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2, T3>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2, T3>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2, T3>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2, c3);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2, T3, T4>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2, T3, T4>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2, T3, T4>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2, T3, T4>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2, T3, T4>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2, c3, c4);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2, T3, T4, T5>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2, T3, T4, T5>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2, T3, T4, T5>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2, T3, T4, T5>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2, T3, T4, T5>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2, c3, c4, c5);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2, T3, T4, T5, T6>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2, T3, T4, T5, T6>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2, T3, T4, T5, T6>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2, T3, T4, T5, T6>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2, c3, c4, c5, c6);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2, T3, T4, T5, T6, T7>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2, T3, T4, T5, T6, T7>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2, c3, c4, c5, c6, c7);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2, T3, T4, T5, T6, T7, T8>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2, c3, c4, c5, c6, c7, c8);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
    public void QueryAddComponent<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(in QueryDescriptor descriptor, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14, in T15 c15)
    {
        var query = GetOrCreateQuery(in descriptor);

        Debug.Assert(!query.Matches(Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.GroupMask));

        foreach (var archetype in query.GetMatchingArchetypesSpan())
        {
            var mask = archetype.Mask | Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.GroupMask;
            if (!groupMaskToArchetype.TryGetValue(mask, out var newArchetype))
            {
                var oldTypes = archetype.ComponentTypes.AsSpan();
                var addTypes = Component<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>.GroupTypeInfo.AsSpan();
#pragma warning disable CA2014 // Do not use stackalloc in loops
                Span<ComponentTypeInfo> newTypes = stackalloc ComponentTypeInfo[oldTypes.Length + addTypes.Length];
#pragma warning restore CA2014 // Do not use stackalloc in loops
                MergeTypes(oldTypes, addTypes, newTypes);
                newArchetype = GetOrCreateArchetype(newTypes);
            }

            var oldArchetypeLastSlot = archetype.CurrentSlot;
            var newArchetypeLastSlot = newArchetype.CurrentSlot;

            var newArchetypeFirstFreeSlot =
                Slot.GetNextFor(newArchetypeLastSlot, newArchetype.EntitiesPerChunk);

            entityDatas.MoveBulk(archetype, oldArchetypeLastSlot,
                                 newArchetype, newArchetypeFirstFreeSlot);

            var oldEntityCapacity = archetype.EntityCapacity;
            newArchetype.EnsureEntityCapacity(archetype.EntityCount + newArchetype.EntityCount);

            Archetype.CopyEntityAndMatchingComponentsAppendBulk(archetype, newArchetype);
            archetype.Clear();

            var setRangeStartSlot = newArchetypeFirstFreeSlot;
            var setRangeEndSlot = newArchetype.CurrentSlot;

            newArchetype.SetRangeBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(setRangeStartSlot, setRangeEndSlot, c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15);

            var newEntityCapacity = newArchetype.EntityCapacity;
            var capacityDelta = newEntityCapacity - oldEntityCapacity;
            entityCapacity += capacityDelta;
        }

        entityDatas.EnsureCapacity(entityCapacity);
    }
#pragma warning restore RCS1242 // Do not pass non-read-only struct by read-only reference
}
