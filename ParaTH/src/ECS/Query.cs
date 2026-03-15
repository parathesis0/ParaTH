using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ParaTH;

[SkipLocalsInit]
public sealed class Query
{
    private readonly ArchetypeList allArchetypes;
    private readonly ArchetypeList matchingArchetypes;

    private int lastVersion;

    private readonly ComponentMask allMask;
    private readonly ComponentMask anyMask;
    private readonly ComponentMask noneMask;
    private readonly ComponentMask exclusiveMask;

    private readonly bool isExclusive;

#pragma warning disable RCS1242 // Do not pass non-read-only struct by read-only reference
    public Query(ArchetypeList allArchetypes, in QueryDescriptor descriptor)
#pragma warning restore RCS1242 // Do not pass non-read-only struct by read-only reference
    {
        this.allArchetypes = allArchetypes;
        matchingArchetypes = new ArchetypeList(8); // todo find a way to pass this in

        lastVersion = -1;

        allMask = descriptor.All;
        anyMask = descriptor.Any == ComponentMask.Zero ? ComponentMask.AllBits : descriptor.Any;
        noneMask = descriptor.None;
        exclusiveMask = descriptor.Exclusive;

        if (exclusiveMask != ComponentMask.Zero)
            isExclusive = true;
    }

    public bool Matches(ComponentMask archetypeMask)
    {
        return isExclusive
            ? archetypeMask == exclusiveMask
            : (archetypeMask & allMask) == allMask &&
              (archetypeMask & anyMask) != ComponentMask.Zero &&
              (archetypeMask & noneMask) == ComponentMask.Zero;
    }

    public Span<Archetype> GetMatchingArchetypesSpan()
    {
        var currentVersion = allArchetypes.Version;

        if (lastVersion == currentVersion)
            return matchingArchetypes.AsSpan();

        lastVersion = currentVersion;

        matchingArchetypes.Clear();
        var archetypeSpan = allArchetypes.AsSpan();

        foreach (var archetype in archetypeSpan)
        {
            if (Matches(archetype.Mask))
                matchingArchetypes.Add(archetype);
        }

        return matchingArchetypes.AsSpan();
    }
}
