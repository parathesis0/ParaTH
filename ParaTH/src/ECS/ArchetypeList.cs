using System;
using System.Collections.Generic;

namespace ParaTH;

// standard list implementation because stability matters for archetype iterating
public sealed class ArchetypeList : IDisposable
{
    private Archetype[] archetypes;
    private readonly int initialCapacity;
    private int count;
    private int version; // self increments when a structual change occurs

    public int Count => count;
    public int Capacity => archetypes.Length;
    public int Version => version; // used to check if all archetype has changed

    public ArchetypeList(int initialCapacity)
    {
        archetypes = [];
        this.initialCapacity = initialCapacity;
        count = 0;
    }

    public void Add(Archetype archetype)
    {
        if (count == archetypes.Length)
            Array.Resize(ref archetypes, count == 0 ? initialCapacity : count * 2);

        archetypes[count++] = archetype;
        version++;
    }

    // removes this item, returns true if success;
    public bool Remove(Archetype archetype)
    {
        int index = Array.IndexOf(archetypes, archetype, 0, count);

        if (index == -1)
            return false;

        Array.Copy(archetypes, index + 1, archetypes, index, count - index - 1);
        archetypes[count - 1] = null!;

        count--;
        version++;
        return true;
    }

    public Span<Archetype> AsSpan()
    {
        return new Span<Archetype>(archetypes, 0, count);
    }

    public Archetype this[int index]
    {
        get => archetypes[index];
        set => archetypes[index] = value;
    }

    public void Clear()
    {
        Array.Clear(archetypes);
        count = 0;
        version++;
    }

    public void Dispose()
    {
        for (int i = 0; i < count; i++)
        {
            archetypes[i].Dispose();
            archetypes[i] = null!;
        }

        count = 0;
        archetypes = [];
        version++;
    }
}
