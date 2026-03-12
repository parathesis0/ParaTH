using System;
using System.Collections.Generic;

namespace ParaTH;

public sealed class Query
{
    private readonly ArchetypeList allArchetypes;
    private readonly ArchetypeList matchingArchetypes;

    private int lastVersion = -1;

    private readonly ulong allMask;
    private readonly ulong anyMask;
    private readonly ulong noneMask;
    private readonly ulong exclusiveMask;

    private readonly bool isExclusive;

#pragma warning disable RCS1242 // Do not pass non-read-only struct by read-only reference
    public Query(ArchetypeList allArchetypes, in QueryDescriptor descriptor)
#pragma warning restore RCS1242 // Do not pass non-read-only struct by read-only reference
    {
        this.allArchetypes = allArchetypes;
        matchingArchetypes = new ArchetypeList(8); // todo find a way to pass this in

        allMask = descriptor.All;
        anyMask = descriptor.Any;
        noneMask = descriptor.None;
        exclusiveMask = descriptor.Exclusive;

        if (exclusiveMask != 0)
            isExclusive = true;
    }

    private bool Match(ulong archetypeMask)
    {
        return isExclusive
            ? archetypeMask == exclusiveMask
            : (archetypeMask & allMask) == allMask &&
              (archetypeMask & anyMask) != 0 &&
              (archetypeMask & noneMask) == 0;
    }

    public Span<Archetype> GetMatchingArchetypes()
    {
        var currentVersion = allArchetypes.Version;

        if (lastVersion == currentVersion)
            return matchingArchetypes.AsSpan();

        lastVersion = currentVersion;

        matchingArchetypes.Clear();
        var archetypeSpan = allArchetypes.AsSpan();

        foreach (var archetype in archetypeSpan)
        {
            if (Match(archetype.Mask))
                matchingArchetypes.Add(archetype);
        }

        return matchingArchetypes.AsSpan();
    }
}
