namespace ParaTH;

public sealed partial class Archetype
{
    public void AddBulk<T0, T1>(Span<Entity> entities, in T0 c0, in T1 c1)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2, T3>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2, in T3 c3)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);
            chunk.GetComponentSpanFull<T3>(out var s3);
            s3.Slice(chunkEntityCount, fillAmount).Fill(c3);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2, T3, T4>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);
            chunk.GetComponentSpanFull<T3>(out var s3);
            s3.Slice(chunkEntityCount, fillAmount).Fill(c3);
            chunk.GetComponentSpanFull<T4>(out var s4);
            s4.Slice(chunkEntityCount, fillAmount).Fill(c4);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);
            chunk.GetComponentSpanFull<T3>(out var s3);
            s3.Slice(chunkEntityCount, fillAmount).Fill(c3);
            chunk.GetComponentSpanFull<T4>(out var s4);
            s4.Slice(chunkEntityCount, fillAmount).Fill(c4);
            chunk.GetComponentSpanFull<T5>(out var s5);
            s5.Slice(chunkEntityCount, fillAmount).Fill(c5);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);
            chunk.GetComponentSpanFull<T3>(out var s3);
            s3.Slice(chunkEntityCount, fillAmount).Fill(c3);
            chunk.GetComponentSpanFull<T4>(out var s4);
            s4.Slice(chunkEntityCount, fillAmount).Fill(c4);
            chunk.GetComponentSpanFull<T5>(out var s5);
            s5.Slice(chunkEntityCount, fillAmount).Fill(c5);
            chunk.GetComponentSpanFull<T6>(out var s6);
            s6.Slice(chunkEntityCount, fillAmount).Fill(c6);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);
            chunk.GetComponentSpanFull<T3>(out var s3);
            s3.Slice(chunkEntityCount, fillAmount).Fill(c3);
            chunk.GetComponentSpanFull<T4>(out var s4);
            s4.Slice(chunkEntityCount, fillAmount).Fill(c4);
            chunk.GetComponentSpanFull<T5>(out var s5);
            s5.Slice(chunkEntityCount, fillAmount).Fill(c5);
            chunk.GetComponentSpanFull<T6>(out var s6);
            s6.Slice(chunkEntityCount, fillAmount).Fill(c6);
            chunk.GetComponentSpanFull<T7>(out var s7);
            s7.Slice(chunkEntityCount, fillAmount).Fill(c7);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);
            chunk.GetComponentSpanFull<T3>(out var s3);
            s3.Slice(chunkEntityCount, fillAmount).Fill(c3);
            chunk.GetComponentSpanFull<T4>(out var s4);
            s4.Slice(chunkEntityCount, fillAmount).Fill(c4);
            chunk.GetComponentSpanFull<T5>(out var s5);
            s5.Slice(chunkEntityCount, fillAmount).Fill(c5);
            chunk.GetComponentSpanFull<T6>(out var s6);
            s6.Slice(chunkEntityCount, fillAmount).Fill(c6);
            chunk.GetComponentSpanFull<T7>(out var s7);
            s7.Slice(chunkEntityCount, fillAmount).Fill(c7);
            chunk.GetComponentSpanFull<T8>(out var s8);
            s8.Slice(chunkEntityCount, fillAmount).Fill(c8);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);
            chunk.GetComponentSpanFull<T3>(out var s3);
            s3.Slice(chunkEntityCount, fillAmount).Fill(c3);
            chunk.GetComponentSpanFull<T4>(out var s4);
            s4.Slice(chunkEntityCount, fillAmount).Fill(c4);
            chunk.GetComponentSpanFull<T5>(out var s5);
            s5.Slice(chunkEntityCount, fillAmount).Fill(c5);
            chunk.GetComponentSpanFull<T6>(out var s6);
            s6.Slice(chunkEntityCount, fillAmount).Fill(c6);
            chunk.GetComponentSpanFull<T7>(out var s7);
            s7.Slice(chunkEntityCount, fillAmount).Fill(c7);
            chunk.GetComponentSpanFull<T8>(out var s8);
            s8.Slice(chunkEntityCount, fillAmount).Fill(c8);
            chunk.GetComponentSpanFull<T9>(out var s9);
            s9.Slice(chunkEntityCount, fillAmount).Fill(c9);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);
            chunk.GetComponentSpanFull<T3>(out var s3);
            s3.Slice(chunkEntityCount, fillAmount).Fill(c3);
            chunk.GetComponentSpanFull<T4>(out var s4);
            s4.Slice(chunkEntityCount, fillAmount).Fill(c4);
            chunk.GetComponentSpanFull<T5>(out var s5);
            s5.Slice(chunkEntityCount, fillAmount).Fill(c5);
            chunk.GetComponentSpanFull<T6>(out var s6);
            s6.Slice(chunkEntityCount, fillAmount).Fill(c6);
            chunk.GetComponentSpanFull<T7>(out var s7);
            s7.Slice(chunkEntityCount, fillAmount).Fill(c7);
            chunk.GetComponentSpanFull<T8>(out var s8);
            s8.Slice(chunkEntityCount, fillAmount).Fill(c8);
            chunk.GetComponentSpanFull<T9>(out var s9);
            s9.Slice(chunkEntityCount, fillAmount).Fill(c9);
            chunk.GetComponentSpanFull<T10>(out var s10);
            s10.Slice(chunkEntityCount, fillAmount).Fill(c10);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);
            chunk.GetComponentSpanFull<T3>(out var s3);
            s3.Slice(chunkEntityCount, fillAmount).Fill(c3);
            chunk.GetComponentSpanFull<T4>(out var s4);
            s4.Slice(chunkEntityCount, fillAmount).Fill(c4);
            chunk.GetComponentSpanFull<T5>(out var s5);
            s5.Slice(chunkEntityCount, fillAmount).Fill(c5);
            chunk.GetComponentSpanFull<T6>(out var s6);
            s6.Slice(chunkEntityCount, fillAmount).Fill(c6);
            chunk.GetComponentSpanFull<T7>(out var s7);
            s7.Slice(chunkEntityCount, fillAmount).Fill(c7);
            chunk.GetComponentSpanFull<T8>(out var s8);
            s8.Slice(chunkEntityCount, fillAmount).Fill(c8);
            chunk.GetComponentSpanFull<T9>(out var s9);
            s9.Slice(chunkEntityCount, fillAmount).Fill(c9);
            chunk.GetComponentSpanFull<T10>(out var s10);
            s10.Slice(chunkEntityCount, fillAmount).Fill(c10);
            chunk.GetComponentSpanFull<T11>(out var s11);
            s11.Slice(chunkEntityCount, fillAmount).Fill(c11);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);
            chunk.GetComponentSpanFull<T3>(out var s3);
            s3.Slice(chunkEntityCount, fillAmount).Fill(c3);
            chunk.GetComponentSpanFull<T4>(out var s4);
            s4.Slice(chunkEntityCount, fillAmount).Fill(c4);
            chunk.GetComponentSpanFull<T5>(out var s5);
            s5.Slice(chunkEntityCount, fillAmount).Fill(c5);
            chunk.GetComponentSpanFull<T6>(out var s6);
            s6.Slice(chunkEntityCount, fillAmount).Fill(c6);
            chunk.GetComponentSpanFull<T7>(out var s7);
            s7.Slice(chunkEntityCount, fillAmount).Fill(c7);
            chunk.GetComponentSpanFull<T8>(out var s8);
            s8.Slice(chunkEntityCount, fillAmount).Fill(c8);
            chunk.GetComponentSpanFull<T9>(out var s9);
            s9.Slice(chunkEntityCount, fillAmount).Fill(c9);
            chunk.GetComponentSpanFull<T10>(out var s10);
            s10.Slice(chunkEntityCount, fillAmount).Fill(c10);
            chunk.GetComponentSpanFull<T11>(out var s11);
            s11.Slice(chunkEntityCount, fillAmount).Fill(c11);
            chunk.GetComponentSpanFull<T12>(out var s12);
            s12.Slice(chunkEntityCount, fillAmount).Fill(c12);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);
            chunk.GetComponentSpanFull<T3>(out var s3);
            s3.Slice(chunkEntityCount, fillAmount).Fill(c3);
            chunk.GetComponentSpanFull<T4>(out var s4);
            s4.Slice(chunkEntityCount, fillAmount).Fill(c4);
            chunk.GetComponentSpanFull<T5>(out var s5);
            s5.Slice(chunkEntityCount, fillAmount).Fill(c5);
            chunk.GetComponentSpanFull<T6>(out var s6);
            s6.Slice(chunkEntityCount, fillAmount).Fill(c6);
            chunk.GetComponentSpanFull<T7>(out var s7);
            s7.Slice(chunkEntityCount, fillAmount).Fill(c7);
            chunk.GetComponentSpanFull<T8>(out var s8);
            s8.Slice(chunkEntityCount, fillAmount).Fill(c8);
            chunk.GetComponentSpanFull<T9>(out var s9);
            s9.Slice(chunkEntityCount, fillAmount).Fill(c9);
            chunk.GetComponentSpanFull<T10>(out var s10);
            s10.Slice(chunkEntityCount, fillAmount).Fill(c10);
            chunk.GetComponentSpanFull<T11>(out var s11);
            s11.Slice(chunkEntityCount, fillAmount).Fill(c11);
            chunk.GetComponentSpanFull<T12>(out var s12);
            s12.Slice(chunkEntityCount, fillAmount).Fill(c12);
            chunk.GetComponentSpanFull<T13>(out var s13);
            s13.Slice(chunkEntityCount, fillAmount).Fill(c13);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);
            chunk.GetComponentSpanFull<T3>(out var s3);
            s3.Slice(chunkEntityCount, fillAmount).Fill(c3);
            chunk.GetComponentSpanFull<T4>(out var s4);
            s4.Slice(chunkEntityCount, fillAmount).Fill(c4);
            chunk.GetComponentSpanFull<T5>(out var s5);
            s5.Slice(chunkEntityCount, fillAmount).Fill(c5);
            chunk.GetComponentSpanFull<T6>(out var s6);
            s6.Slice(chunkEntityCount, fillAmount).Fill(c6);
            chunk.GetComponentSpanFull<T7>(out var s7);
            s7.Slice(chunkEntityCount, fillAmount).Fill(c7);
            chunk.GetComponentSpanFull<T8>(out var s8);
            s8.Slice(chunkEntityCount, fillAmount).Fill(c8);
            chunk.GetComponentSpanFull<T9>(out var s9);
            s9.Slice(chunkEntityCount, fillAmount).Fill(c9);
            chunk.GetComponentSpanFull<T10>(out var s10);
            s10.Slice(chunkEntityCount, fillAmount).Fill(c10);
            chunk.GetComponentSpanFull<T11>(out var s11);
            s11.Slice(chunkEntityCount, fillAmount).Fill(c11);
            chunk.GetComponentSpanFull<T12>(out var s12);
            s12.Slice(chunkEntityCount, fillAmount).Fill(c12);
            chunk.GetComponentSpanFull<T13>(out var s13);
            s13.Slice(chunkEntityCount, fillAmount).Fill(c13);
            chunk.GetComponentSpanFull<T14>(out var s14);
            s14.Slice(chunkEntityCount, fillAmount).Fill(c14);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
    public void AddBulk<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Span<Entity> entities, in T0 c0, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6, in T7 c7, in T8 c8, in T9 c9, in T10 c10, in T11 c11, in T12 c12, in T13 c13, in T14 c14, in T15 c15)
    {
        var totalAmount = entities.Length;
        var created = 0;
        var chunkIndexIncrement = 0;

        for (int i = CurrentChunkIndex; i < chunks.Count; i++)
        {
            ref var chunk = ref chunks[i];
            var chunkEntityCount = chunk.EntityCount;
            var chunkCapacity = chunk.Capacity;

            var fillAmount = Math.Min(chunkCapacity - chunkEntityCount, totalAmount - created);

            var src = entities.Slice(created, fillAmount);
            var dst = chunk.Entities.AsSpan(chunkEntityCount, fillAmount);
            src.CopyTo(dst);

            chunk.GetComponentSpanFull<T0>(out var s0);
            s0.Slice(chunkEntityCount, fillAmount).Fill(c0);
            chunk.GetComponentSpanFull<T1>(out var s1);
            s1.Slice(chunkEntityCount, fillAmount).Fill(c1);
            chunk.GetComponentSpanFull<T2>(out var s2);
            s2.Slice(chunkEntityCount, fillAmount).Fill(c2);
            chunk.GetComponentSpanFull<T3>(out var s3);
            s3.Slice(chunkEntityCount, fillAmount).Fill(c3);
            chunk.GetComponentSpanFull<T4>(out var s4);
            s4.Slice(chunkEntityCount, fillAmount).Fill(c4);
            chunk.GetComponentSpanFull<T5>(out var s5);
            s5.Slice(chunkEntityCount, fillAmount).Fill(c5);
            chunk.GetComponentSpanFull<T6>(out var s6);
            s6.Slice(chunkEntityCount, fillAmount).Fill(c6);
            chunk.GetComponentSpanFull<T7>(out var s7);
            s7.Slice(chunkEntityCount, fillAmount).Fill(c7);
            chunk.GetComponentSpanFull<T8>(out var s8);
            s8.Slice(chunkEntityCount, fillAmount).Fill(c8);
            chunk.GetComponentSpanFull<T9>(out var s9);
            s9.Slice(chunkEntityCount, fillAmount).Fill(c9);
            chunk.GetComponentSpanFull<T10>(out var s10);
            s10.Slice(chunkEntityCount, fillAmount).Fill(c10);
            chunk.GetComponentSpanFull<T11>(out var s11);
            s11.Slice(chunkEntityCount, fillAmount).Fill(c11);
            chunk.GetComponentSpanFull<T12>(out var s12);
            s12.Slice(chunkEntityCount, fillAmount).Fill(c12);
            chunk.GetComponentSpanFull<T13>(out var s13);
            s13.Slice(chunkEntityCount, fillAmount).Fill(c13);
            chunk.GetComponentSpanFull<T14>(out var s14);
            s14.Slice(chunkEntityCount, fillAmount).Fill(c14);
            chunk.GetComponentSpanFull<T15>(out var s15);
            s15.Slice(chunkEntityCount, fillAmount).Fill(c15);

            chunkEntityCount += fillAmount;
            created += fillAmount;

            if (chunkEntityCount == chunkCapacity)
                chunkIndexIncrement++;

            chunk.EntityCount = chunkEntityCount;

            if (created >= totalAmount)
                break;
        }

        this.EntityCount += totalAmount;
        this.CurrentChunkIndex += chunkIndexIncrement;
    }
}
