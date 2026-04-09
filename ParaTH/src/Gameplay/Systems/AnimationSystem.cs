namespace ParaTH;

using static AnimationAsset;

// updates counters
public sealed class AnimationSystem(World world)
{
    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAny<SpawnEffect, AnimationRenderer>();

    public void Update()
    {
        var q = world.GetOrCreateQuery(descriptor);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool hasAni = archetype.Has<AnimationRenderer>();
            bool hasSpw = archetype.Has<SpawnEffect>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                var aniSpan = hasAni ? chunk.GetFilledComponentSpan<AnimationRenderer>() : default;
                var spwSpan = hasSpw ? chunk.GetFilledComponentSpan<SpawnEffect>() : default;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    if (hasSpw)
                        UpdateSpawnEffect(ref spwSpan.UnsafeAt(i));

                    if (hasAni)
                        UpdateAnimation(ref aniSpan.UnsafeAt(i));
                }
            }
        }
    }

    private static void UpdateSpawnEffect(ref SpawnEffect spawnAnim)
    {
        if (spawnAnim.IsPlaying)
            spawnAnim.Counter++;
    }

    private static void UpdateAnimation(ref AnimationRenderer anim)
    {
        if (!anim.IsPlaying)
            return;

        var frames = anim.Animation.Frames;
        int len = frames.Length;
        int frameIndex = anim.FrameIndex;
        int counter = anim.Counter;

        if (!anim.IsReverse)
        {
            counter++;
            while (counter >= frames.UnsafeAt(frameIndex).FrameDuration)
            {
                frameIndex++;
                if (frameIndex >= len) break;
                counter = 0;
            }

            if (frameIndex >= len)
            {
                switch (anim.Animation.Type)
                {
                    case PlayType.Hold:
                        anim.IsPlaying = false;
                        frameIndex = len - 1;
                        break;
                    case PlayType.Loop:
                        frameIndex = 0;
                        counter = 0;
                        break;
                    case PlayType.PingPong:
                        anim.IsReverse = true;
                        frameIndex = Math.Max(len - 2, 0);
                        counter = frames.UnsafeAt(frameIndex).FrameDuration;
                        break;
                }
            }
        }
        else
        {
            counter--;
            while (counter <= 0)
            {
                frameIndex--;
                if (frameIndex < 0) break;
                counter = frames.UnsafeAt(frameIndex).FrameDuration;
            }

            if (frameIndex < 0)
            {
                switch (anim.Animation.Type)
                {
                    case PlayType.Hold:
                        anim.IsPlaying = false;
                        frameIndex = 0;
                        counter = 0;
                        break;
                    case PlayType.Loop:
                        frameIndex = len - 1;
                        counter = frames.UnsafeAt(frameIndex).FrameDuration;
                        break;
                    case PlayType.PingPong:
                        anim.IsReverse = false;
                        frameIndex = Math.Min(1, len - 1);
                        counter = 0;
                        break;
                }
            }
        }

        anim.FrameIndex = (ushort)frameIndex;
        anim.Counter = counter;
    }
}

