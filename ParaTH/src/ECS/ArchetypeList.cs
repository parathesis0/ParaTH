using System;
using System.Collections.Generic;

namespace ParaTH;

// standard list implementation because stability matters for archetype iterating
public sealed class ArchetypeList : IDisposable
{
    private Archetype[] Archetypes;
    private readonly int initialCapacity;
    private int count;
    private int version; // self increments when an structual change occurs

    public int Count => count;
    public int Capacity => Archetypes.Length;
    public int Version => version; // used to check if all archetype has changed

    public ArchetypeList(int initialCapacity)
    {
        Archetypes = [];
        this.initialCapacity = initialCapacity;
        count = 0;
    }

    public void Add(Archetype archetype)
    {
        if (count == Archetypes.Length)
            Array.Resize(ref Archetypes, count == 0 ? initialCapacity : count * 2);

        Archetypes[count++] = archetype;
        version++;
    }

    // removes this item, returns true if success;
    public bool Remove(Archetype archetype)
    {
        int index = Array.IndexOf(Archetypes, archetype, 0, count);

        if (index == -1)
            return false;

        Array.Copy(Archetypes, index + 1, Archetypes, index, count - index - 1);

        count--;
        version++;
        return true;
    }

    public Span<Archetype> AsSpan()
    {
        return new Span<Archetype>(Archetypes, 0, count);
    }

    public Archetype this[int index]
    {
        get => Archetypes[index];
        set => Archetypes[index] = value;
    }

    public void Clear()
    {
        Array.Clear(Archetypes);
        count = 0;
        version++;
    }

    public void Dispose()
    {
        Clear();
    }
}
