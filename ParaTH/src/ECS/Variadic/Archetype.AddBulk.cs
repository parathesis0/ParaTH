namespace ParaTH;

public sealed partial class Archetype
{
    public void AddBulk<T0, T1>(Span<Entity> entities, Span<T0> c0, Span<T1> c1)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2, T3>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);
            chunk.GetFullComponentSpan<T3>(out var s3);
            var srcC3 = c3.Slice(created, fillAmount);
            var dstC3 = s3.Slice(chunkEntityCount, fillAmount);
            srcC3.CopyTo(dstC3);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2, T3, T4>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);
            chunk.GetFullComponentSpan<T3>(out var s3);
            var srcC3 = c3.Slice(created, fillAmount);
            var dstC3 = s3.Slice(chunkEntityCount, fillAmount);
            srcC3.CopyTo(dstC3);
            chunk.GetFullComponentSpan<T4>(out var s4);
            var srcC4 = c4.Slice(created, fillAmount);
            var dstC4 = s4.Slice(chunkEntityCount, fillAmount);
            srcC4.CopyTo(dstC4);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);
            chunk.GetFullComponentSpan<T3>(out var s3);
            var srcC3 = c3.Slice(created, fillAmount);
            var dstC3 = s3.Slice(chunkEntityCount, fillAmount);
            srcC3.CopyTo(dstC3);
            chunk.GetFullComponentSpan<T4>(out var s4);
            var srcC4 = c4.Slice(created, fillAmount);
            var dstC4 = s4.Slice(chunkEntityCount, fillAmount);
            srcC4.CopyTo(dstC4);
            chunk.GetFullComponentSpan<T5>(out var s5);
            var srcC5 = c5.Slice(created, fillAmount);
            var dstC5 = s5.Slice(chunkEntityCount, fillAmount);
            srcC5.CopyTo(dstC5);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);
            chunk.GetFullComponentSpan<T3>(out var s3);
            var srcC3 = c3.Slice(created, fillAmount);
            var dstC3 = s3.Slice(chunkEntityCount, fillAmount);
            srcC3.CopyTo(dstC3);
            chunk.GetFullComponentSpan<T4>(out var s4);
            var srcC4 = c4.Slice(created, fillAmount);
            var dstC4 = s4.Slice(chunkEntityCount, fillAmount);
            srcC4.CopyTo(dstC4);
            chunk.GetFullComponentSpan<T5>(out var s5);
            var srcC5 = c5.Slice(created, fillAmount);
            var dstC5 = s5.Slice(chunkEntityCount, fillAmount);
            srcC5.CopyTo(dstC5);
            chunk.GetFullComponentSpan<T6>(out var s6);
            var srcC6 = c6.Slice(created, fillAmount);
            var dstC6 = s6.Slice(chunkEntityCount, fillAmount);
            srcC6.CopyTo(dstC6);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);
            chunk.GetFullComponentSpan<T3>(out var s3);
            var srcC3 = c3.Slice(created, fillAmount);
            var dstC3 = s3.Slice(chunkEntityCount, fillAmount);
            srcC3.CopyTo(dstC3);
            chunk.GetFullComponentSpan<T4>(out var s4);
            var srcC4 = c4.Slice(created, fillAmount);
            var dstC4 = s4.Slice(chunkEntityCount, fillAmount);
            srcC4.CopyTo(dstC4);
            chunk.GetFullComponentSpan<T5>(out var s5);
            var srcC5 = c5.Slice(created, fillAmount);
            var dstC5 = s5.Slice(chunkEntityCount, fillAmount);
            srcC5.CopyTo(dstC5);
            chunk.GetFullComponentSpan<T6>(out var s6);
            var srcC6 = c6.Slice(created, fillAmount);
            var dstC6 = s6.Slice(chunkEntityCount, fillAmount);
            srcC6.CopyTo(dstC6);
            chunk.GetFullComponentSpan<T7>(out var s7);
            var srcC7 = c7.Slice(created, fillAmount);
            var dstC7 = s7.Slice(chunkEntityCount, fillAmount);
            srcC7.CopyTo(dstC7);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);
            chunk.GetFullComponentSpan<T3>(out var s3);
            var srcC3 = c3.Slice(created, fillAmount);
            var dstC3 = s3.Slice(chunkEntityCount, fillAmount);
            srcC3.CopyTo(dstC3);
            chunk.GetFullComponentSpan<T4>(out var s4);
            var srcC4 = c4.Slice(created, fillAmount);
            var dstC4 = s4.Slice(chunkEntityCount, fillAmount);
            srcC4.CopyTo(dstC4);
            chunk.GetFullComponentSpan<T5>(out var s5);
            var srcC5 = c5.Slice(created, fillAmount);
            var dstC5 = s5.Slice(chunkEntityCount, fillAmount);
            srcC5.CopyTo(dstC5);
            chunk.GetFullComponentSpan<T6>(out var s6);
            var srcC6 = c6.Slice(created, fillAmount);
            var dstC6 = s6.Slice(chunkEntityCount, fillAmount);
            srcC6.CopyTo(dstC6);
            chunk.GetFullComponentSpan<T7>(out var s7);
            var srcC7 = c7.Slice(created, fillAmount);
            var dstC7 = s7.Slice(chunkEntityCount, fillAmount);
            srcC7.CopyTo(dstC7);
            chunk.GetFullComponentSpan<T8>(out var s8);
            var srcC8 = c8.Slice(created, fillAmount);
            var dstC8 = s8.Slice(chunkEntityCount, fillAmount);
            srcC8.CopyTo(dstC8);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);
            chunk.GetFullComponentSpan<T3>(out var s3);
            var srcC3 = c3.Slice(created, fillAmount);
            var dstC3 = s3.Slice(chunkEntityCount, fillAmount);
            srcC3.CopyTo(dstC3);
            chunk.GetFullComponentSpan<T4>(out var s4);
            var srcC4 = c4.Slice(created, fillAmount);
            var dstC4 = s4.Slice(chunkEntityCount, fillAmount);
            srcC4.CopyTo(dstC4);
            chunk.GetFullComponentSpan<T5>(out var s5);
            var srcC5 = c5.Slice(created, fillAmount);
            var dstC5 = s5.Slice(chunkEntityCount, fillAmount);
            srcC5.CopyTo(dstC5);
            chunk.GetFullComponentSpan<T6>(out var s6);
            var srcC6 = c6.Slice(created, fillAmount);
            var dstC6 = s6.Slice(chunkEntityCount, fillAmount);
            srcC6.CopyTo(dstC6);
            chunk.GetFullComponentSpan<T7>(out var s7);
            var srcC7 = c7.Slice(created, fillAmount);
            var dstC7 = s7.Slice(chunkEntityCount, fillAmount);
            srcC7.CopyTo(dstC7);
            chunk.GetFullComponentSpan<T8>(out var s8);
            var srcC8 = c8.Slice(created, fillAmount);
            var dstC8 = s8.Slice(chunkEntityCount, fillAmount);
            srcC8.CopyTo(dstC8);
            chunk.GetFullComponentSpan<T9>(out var s9);
            var srcC9 = c9.Slice(created, fillAmount);
            var dstC9 = s9.Slice(chunkEntityCount, fillAmount);
            srcC9.CopyTo(dstC9);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9, Span<T10> c10)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);
            chunk.GetFullComponentSpan<T3>(out var s3);
            var srcC3 = c3.Slice(created, fillAmount);
            var dstC3 = s3.Slice(chunkEntityCount, fillAmount);
            srcC3.CopyTo(dstC3);
            chunk.GetFullComponentSpan<T4>(out var s4);
            var srcC4 = c4.Slice(created, fillAmount);
            var dstC4 = s4.Slice(chunkEntityCount, fillAmount);
            srcC4.CopyTo(dstC4);
            chunk.GetFullComponentSpan<T5>(out var s5);
            var srcC5 = c5.Slice(created, fillAmount);
            var dstC5 = s5.Slice(chunkEntityCount, fillAmount);
            srcC5.CopyTo(dstC5);
            chunk.GetFullComponentSpan<T6>(out var s6);
            var srcC6 = c6.Slice(created, fillAmount);
            var dstC6 = s6.Slice(chunkEntityCount, fillAmount);
            srcC6.CopyTo(dstC6);
            chunk.GetFullComponentSpan<T7>(out var s7);
            var srcC7 = c7.Slice(created, fillAmount);
            var dstC7 = s7.Slice(chunkEntityCount, fillAmount);
            srcC7.CopyTo(dstC7);
            chunk.GetFullComponentSpan<T8>(out var s8);
            var srcC8 = c8.Slice(created, fillAmount);
            var dstC8 = s8.Slice(chunkEntityCount, fillAmount);
            srcC8.CopyTo(dstC8);
            chunk.GetFullComponentSpan<T9>(out var s9);
            var srcC9 = c9.Slice(created, fillAmount);
            var dstC9 = s9.Slice(chunkEntityCount, fillAmount);
            srcC9.CopyTo(dstC9);
            chunk.GetFullComponentSpan<T10>(out var s10);
            var srcC10 = c10.Slice(created, fillAmount);
            var dstC10 = s10.Slice(chunkEntityCount, fillAmount);
            srcC10.CopyTo(dstC10);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9, Span<T10> c10, Span<T11> c11)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);
            chunk.GetFullComponentSpan<T3>(out var s3);
            var srcC3 = c3.Slice(created, fillAmount);
            var dstC3 = s3.Slice(chunkEntityCount, fillAmount);
            srcC3.CopyTo(dstC3);
            chunk.GetFullComponentSpan<T4>(out var s4);
            var srcC4 = c4.Slice(created, fillAmount);
            var dstC4 = s4.Slice(chunkEntityCount, fillAmount);
            srcC4.CopyTo(dstC4);
            chunk.GetFullComponentSpan<T5>(out var s5);
            var srcC5 = c5.Slice(created, fillAmount);
            var dstC5 = s5.Slice(chunkEntityCount, fillAmount);
            srcC5.CopyTo(dstC5);
            chunk.GetFullComponentSpan<T6>(out var s6);
            var srcC6 = c6.Slice(created, fillAmount);
            var dstC6 = s6.Slice(chunkEntityCount, fillAmount);
            srcC6.CopyTo(dstC6);
            chunk.GetFullComponentSpan<T7>(out var s7);
            var srcC7 = c7.Slice(created, fillAmount);
            var dstC7 = s7.Slice(chunkEntityCount, fillAmount);
            srcC7.CopyTo(dstC7);
            chunk.GetFullComponentSpan<T8>(out var s8);
            var srcC8 = c8.Slice(created, fillAmount);
            var dstC8 = s8.Slice(chunkEntityCount, fillAmount);
            srcC8.CopyTo(dstC8);
            chunk.GetFullComponentSpan<T9>(out var s9);
            var srcC9 = c9.Slice(created, fillAmount);
            var dstC9 = s9.Slice(chunkEntityCount, fillAmount);
            srcC9.CopyTo(dstC9);
            chunk.GetFullComponentSpan<T10>(out var s10);
            var srcC10 = c10.Slice(created, fillAmount);
            var dstC10 = s10.Slice(chunkEntityCount, fillAmount);
            srcC10.CopyTo(dstC10);
            chunk.GetFullComponentSpan<T11>(out var s11);
            var srcC11 = c11.Slice(created, fillAmount);
            var dstC11 = s11.Slice(chunkEntityCount, fillAmount);
            srcC11.CopyTo(dstC11);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9, Span<T10> c10, Span<T11> c11, Span<T12> c12)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);
            chunk.GetFullComponentSpan<T3>(out var s3);
            var srcC3 = c3.Slice(created, fillAmount);
            var dstC3 = s3.Slice(chunkEntityCount, fillAmount);
            srcC3.CopyTo(dstC3);
            chunk.GetFullComponentSpan<T4>(out var s4);
            var srcC4 = c4.Slice(created, fillAmount);
            var dstC4 = s4.Slice(chunkEntityCount, fillAmount);
            srcC4.CopyTo(dstC4);
            chunk.GetFullComponentSpan<T5>(out var s5);
            var srcC5 = c5.Slice(created, fillAmount);
            var dstC5 = s5.Slice(chunkEntityCount, fillAmount);
            srcC5.CopyTo(dstC5);
            chunk.GetFullComponentSpan<T6>(out var s6);
            var srcC6 = c6.Slice(created, fillAmount);
            var dstC6 = s6.Slice(chunkEntityCount, fillAmount);
            srcC6.CopyTo(dstC6);
            chunk.GetFullComponentSpan<T7>(out var s7);
            var srcC7 = c7.Slice(created, fillAmount);
            var dstC7 = s7.Slice(chunkEntityCount, fillAmount);
            srcC7.CopyTo(dstC7);
            chunk.GetFullComponentSpan<T8>(out var s8);
            var srcC8 = c8.Slice(created, fillAmount);
            var dstC8 = s8.Slice(chunkEntityCount, fillAmount);
            srcC8.CopyTo(dstC8);
            chunk.GetFullComponentSpan<T9>(out var s9);
            var srcC9 = c9.Slice(created, fillAmount);
            var dstC9 = s9.Slice(chunkEntityCount, fillAmount);
            srcC9.CopyTo(dstC9);
            chunk.GetFullComponentSpan<T10>(out var s10);
            var srcC10 = c10.Slice(created, fillAmount);
            var dstC10 = s10.Slice(chunkEntityCount, fillAmount);
            srcC10.CopyTo(dstC10);
            chunk.GetFullComponentSpan<T11>(out var s11);
            var srcC11 = c11.Slice(created, fillAmount);
            var dstC11 = s11.Slice(chunkEntityCount, fillAmount);
            srcC11.CopyTo(dstC11);
            chunk.GetFullComponentSpan<T12>(out var s12);
            var srcC12 = c12.Slice(created, fillAmount);
            var dstC12 = s12.Slice(chunkEntityCount, fillAmount);
            srcC12.CopyTo(dstC12);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9, Span<T10> c10, Span<T11> c11, Span<T12> c12, Span<T13> c13)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);
            chunk.GetFullComponentSpan<T3>(out var s3);
            var srcC3 = c3.Slice(created, fillAmount);
            var dstC3 = s3.Slice(chunkEntityCount, fillAmount);
            srcC3.CopyTo(dstC3);
            chunk.GetFullComponentSpan<T4>(out var s4);
            var srcC4 = c4.Slice(created, fillAmount);
            var dstC4 = s4.Slice(chunkEntityCount, fillAmount);
            srcC4.CopyTo(dstC4);
            chunk.GetFullComponentSpan<T5>(out var s5);
            var srcC5 = c5.Slice(created, fillAmount);
            var dstC5 = s5.Slice(chunkEntityCount, fillAmount);
            srcC5.CopyTo(dstC5);
            chunk.GetFullComponentSpan<T6>(out var s6);
            var srcC6 = c6.Slice(created, fillAmount);
            var dstC6 = s6.Slice(chunkEntityCount, fillAmount);
            srcC6.CopyTo(dstC6);
            chunk.GetFullComponentSpan<T7>(out var s7);
            var srcC7 = c7.Slice(created, fillAmount);
            var dstC7 = s7.Slice(chunkEntityCount, fillAmount);
            srcC7.CopyTo(dstC7);
            chunk.GetFullComponentSpan<T8>(out var s8);
            var srcC8 = c8.Slice(created, fillAmount);
            var dstC8 = s8.Slice(chunkEntityCount, fillAmount);
            srcC8.CopyTo(dstC8);
            chunk.GetFullComponentSpan<T9>(out var s9);
            var srcC9 = c9.Slice(created, fillAmount);
            var dstC9 = s9.Slice(chunkEntityCount, fillAmount);
            srcC9.CopyTo(dstC9);
            chunk.GetFullComponentSpan<T10>(out var s10);
            var srcC10 = c10.Slice(created, fillAmount);
            var dstC10 = s10.Slice(chunkEntityCount, fillAmount);
            srcC10.CopyTo(dstC10);
            chunk.GetFullComponentSpan<T11>(out var s11);
            var srcC11 = c11.Slice(created, fillAmount);
            var dstC11 = s11.Slice(chunkEntityCount, fillAmount);
            srcC11.CopyTo(dstC11);
            chunk.GetFullComponentSpan<T12>(out var s12);
            var srcC12 = c12.Slice(created, fillAmount);
            var dstC12 = s12.Slice(chunkEntityCount, fillAmount);
            srcC12.CopyTo(dstC12);
            chunk.GetFullComponentSpan<T13>(out var s13);
            var srcC13 = c13.Slice(created, fillAmount);
            var dstC13 = s13.Slice(chunkEntityCount, fillAmount);
            srcC13.CopyTo(dstC13);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9, Span<T10> c10, Span<T11> c11, Span<T12> c12, Span<T13> c13, Span<T14> c14)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);
            chunk.GetFullComponentSpan<T3>(out var s3);
            var srcC3 = c3.Slice(created, fillAmount);
            var dstC3 = s3.Slice(chunkEntityCount, fillAmount);
            srcC3.CopyTo(dstC3);
            chunk.GetFullComponentSpan<T4>(out var s4);
            var srcC4 = c4.Slice(created, fillAmount);
            var dstC4 = s4.Slice(chunkEntityCount, fillAmount);
            srcC4.CopyTo(dstC4);
            chunk.GetFullComponentSpan<T5>(out var s5);
            var srcC5 = c5.Slice(created, fillAmount);
            var dstC5 = s5.Slice(chunkEntityCount, fillAmount);
            srcC5.CopyTo(dstC5);
            chunk.GetFullComponentSpan<T6>(out var s6);
            var srcC6 = c6.Slice(created, fillAmount);
            var dstC6 = s6.Slice(chunkEntityCount, fillAmount);
            srcC6.CopyTo(dstC6);
            chunk.GetFullComponentSpan<T7>(out var s7);
            var srcC7 = c7.Slice(created, fillAmount);
            var dstC7 = s7.Slice(chunkEntityCount, fillAmount);
            srcC7.CopyTo(dstC7);
            chunk.GetFullComponentSpan<T8>(out var s8);
            var srcC8 = c8.Slice(created, fillAmount);
            var dstC8 = s8.Slice(chunkEntityCount, fillAmount);
            srcC8.CopyTo(dstC8);
            chunk.GetFullComponentSpan<T9>(out var s9);
            var srcC9 = c9.Slice(created, fillAmount);
            var dstC9 = s9.Slice(chunkEntityCount, fillAmount);
            srcC9.CopyTo(dstC9);
            chunk.GetFullComponentSpan<T10>(out var s10);
            var srcC10 = c10.Slice(created, fillAmount);
            var dstC10 = s10.Slice(chunkEntityCount, fillAmount);
            srcC10.CopyTo(dstC10);
            chunk.GetFullComponentSpan<T11>(out var s11);
            var srcC11 = c11.Slice(created, fillAmount);
            var dstC11 = s11.Slice(chunkEntityCount, fillAmount);
            srcC11.CopyTo(dstC11);
            chunk.GetFullComponentSpan<T12>(out var s12);
            var srcC12 = c12.Slice(created, fillAmount);
            var dstC12 = s12.Slice(chunkEntityCount, fillAmount);
            srcC12.CopyTo(dstC12);
            chunk.GetFullComponentSpan<T13>(out var s13);
            var srcC13 = c13.Slice(created, fillAmount);
            var dstC13 = s13.Slice(chunkEntityCount, fillAmount);
            srcC13.CopyTo(dstC13);
            chunk.GetFullComponentSpan<T14>(out var s14);
            var srcC14 = c14.Slice(created, fillAmount);
            var dstC14 = s14.Slice(chunkEntityCount, fillAmount);
            srcC14.CopyTo(dstC14);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Span<Entity> entities, Span<T0> c0, Span<T1> c1, Span<T2> c2, Span<T3> c3, Span<T4> c4, Span<T5> c5, Span<T6> c6, Span<T7> c7, Span<T8> c8, Span<T9> c9, Span<T10> c10, Span<T11> c11, Span<T12> c12, Span<T13> c13, Span<T14> c14, Span<T15> c15)
    {
        var totalAmount = entities.Length;
        var created = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.EntityCapacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetFullComponentSpan<T0>(out var s0);
            var srcC0 = c0.Slice(created, fillAmount);
            var dstC0 = s0.Slice(chunkEntityCount, fillAmount);
            srcC0.CopyTo(dstC0);
            chunk.GetFullComponentSpan<T1>(out var s1);
            var srcC1 = c1.Slice(created, fillAmount);
            var dstC1 = s1.Slice(chunkEntityCount, fillAmount);
            srcC1.CopyTo(dstC1);
            chunk.GetFullComponentSpan<T2>(out var s2);
            var srcC2 = c2.Slice(created, fillAmount);
            var dstC2 = s2.Slice(chunkEntityCount, fillAmount);
            srcC2.CopyTo(dstC2);
            chunk.GetFullComponentSpan<T3>(out var s3);
            var srcC3 = c3.Slice(created, fillAmount);
            var dstC3 = s3.Slice(chunkEntityCount, fillAmount);
            srcC3.CopyTo(dstC3);
            chunk.GetFullComponentSpan<T4>(out var s4);
            var srcC4 = c4.Slice(created, fillAmount);
            var dstC4 = s4.Slice(chunkEntityCount, fillAmount);
            srcC4.CopyTo(dstC4);
            chunk.GetFullComponentSpan<T5>(out var s5);
            var srcC5 = c5.Slice(created, fillAmount);
            var dstC5 = s5.Slice(chunkEntityCount, fillAmount);
            srcC5.CopyTo(dstC5);
            chunk.GetFullComponentSpan<T6>(out var s6);
            var srcC6 = c6.Slice(created, fillAmount);
            var dstC6 = s6.Slice(chunkEntityCount, fillAmount);
            srcC6.CopyTo(dstC6);
            chunk.GetFullComponentSpan<T7>(out var s7);
            var srcC7 = c7.Slice(created, fillAmount);
            var dstC7 = s7.Slice(chunkEntityCount, fillAmount);
            srcC7.CopyTo(dstC7);
            chunk.GetFullComponentSpan<T8>(out var s8);
            var srcC8 = c8.Slice(created, fillAmount);
            var dstC8 = s8.Slice(chunkEntityCount, fillAmount);
            srcC8.CopyTo(dstC8);
            chunk.GetFullComponentSpan<T9>(out var s9);
            var srcC9 = c9.Slice(created, fillAmount);
            var dstC9 = s9.Slice(chunkEntityCount, fillAmount);
            srcC9.CopyTo(dstC9);
            chunk.GetFullComponentSpan<T10>(out var s10);
            var srcC10 = c10.Slice(created, fillAmount);
            var dstC10 = s10.Slice(chunkEntityCount, fillAmount);
            srcC10.CopyTo(dstC10);
            chunk.GetFullComponentSpan<T11>(out var s11);
            var srcC11 = c11.Slice(created, fillAmount);
            var dstC11 = s11.Slice(chunkEntityCount, fillAmount);
            srcC11.CopyTo(dstC11);
            chunk.GetFullComponentSpan<T12>(out var s12);
            var srcC12 = c12.Slice(created, fillAmount);
            var dstC12 = s12.Slice(chunkEntityCount, fillAmount);
            srcC12.CopyTo(dstC12);
            chunk.GetFullComponentSpan<T13>(out var s13);
            var srcC13 = c13.Slice(created, fillAmount);
            var dstC13 = s13.Slice(chunkEntityCount, fillAmount);
            srcC13.CopyTo(dstC13);
            chunk.GetFullComponentSpan<T14>(out var s14);
            var srcC14 = c14.Slice(created, fillAmount);
            var dstC14 = s14.Slice(chunkEntityCount, fillAmount);
            srcC14.CopyTo(dstC14);
            chunk.GetFullComponentSpan<T15>(out var s15);
            var srcC15 = c15.Slice(created, fillAmount);
            var dstC15 = s15.Slice(chunkEntityCount, fillAmount);
            srcC15.CopyTo(dstC15);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
            {
                this.CurrentChunkIndex = i;
                break;
            }
        }

        this.EntityCount += totalAmount;
    }
}
