namespace ParaTH;

using static AnimationAsset;

// updates counters and apply animation to renderer
public sealed class AnimationSystem(World world)
{
    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Renderer>()
        .WithAny<SpawnEffect, SpriteAnimator, WalkAnimator>();

    public void Update()
    {
        var q = world.GetOrCreateQuery(descriptor);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            bool hasAni = archetype.Has<SpriteAnimator>();
            bool hasSpw = archetype.Has<SpawnEffect>();
            bool hasWlk = archetype.Has<WalkAnimator>();

            foreach (ref var chunk in archetype.GetChunksSpan())
            {
                chunk.GetFilledComponentSpan<Renderer>(out var rndSpan);

                var aniSpan = hasAni ? chunk.GetFilledComponentSpan<SpriteAnimator>() : default;
                var spwSpan = hasSpw ? chunk.GetFilledComponentSpan<SpawnEffect>() : default;
                var wlkSpan = hasWlk ? chunk.GetFilledComponentSpan<WalkAnimator>() : default;

                for (int i = 0; i < chunk.EntityCount; i++)
                {
                    if (hasSpw)
                        UpdateSpawnEffect(ref spwSpan.UnsafeAt(i));

                    if (hasAni)
                    {
                        ref var renderer = ref rndSpan.UnsafeAt(i);
                        ref var animator = ref aniSpan.UnsafeAt(i);

                        UpdateSpriteAnimation(ref animator);

                        renderer.Texture = animator.Animation.Texture;
                        renderer.SourceRect = animator.CurrentFrame.SourceRect;
                        renderer.Anchor = animator.CurrentFrame.Anchor;
                    }

                    if (hasWlk)
                    {
                        ref var renderer = ref rndSpan.UnsafeAt(i);
                        ref var walkAnim = ref wlkSpan.UnsafeAt(i);

                        UpdateWalkAnimation(ref walkAnim);

                        renderer.Texture = walkAnim.CurrentAnimation.Texture;
                        renderer.SourceRect = walkAnim.CurrentFrame.SourceRect;
                        renderer.Anchor = walkAnim.CurrentFrame.Anchor;
                    }
                }
            }
        }
    }

    private static void UpdateSpawnEffect(ref SpawnEffect spawnAnim)
    {
        if (spawnAnim.IsPlaying)
            spawnAnim.Counter++;
    }

    private static void UpdateSpriteAnimation(ref SpriteAnimator anim)
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

    private static void UpdateWalkAnimation(ref WalkAnimator walk)
    {
        var dir = walk.CurrentDirection;

        switch (walk.CurrentState)
        {
            case WalkAnimator.State.IdleLoop:
                if (dir < 0)
                    TransitionTo(ref walk, WalkAnimator.State.TransitionLeftLoop, reverse: false);
                else if (dir > 0)
                    TransitionTo(ref walk, WalkAnimator.State.TransitionRightLoop, reverse: false);
                break;

            case WalkAnimator.State.LeftLoop:
                if (dir >= 0)
                    TransitionTo(ref walk, WalkAnimator.State.TransitionLeftLoop, reverse: true);
                break;

            case WalkAnimator.State.RightLoop:
                if (dir <= 0)
                    TransitionTo(ref walk, WalkAnimator.State.TransitionRightLoop, reverse: true);
                break;

            case WalkAnimator.State.TransitionLeftLoop:
                if (!walk.IsReverse && dir >= 0)
                    walk.IsReverse = true;
                else if (walk.IsReverse && dir < 0)
                    walk.IsReverse = false;
                break;

            case WalkAnimator.State.TransitionRightLoop:
                if (!walk.IsReverse && dir <= 0)
                    walk.IsReverse = true;
                else if (walk.IsReverse && dir > 0)
                    walk.IsReverse = false;
                break;
        }

        AdvanceFrame(ref walk);
    }

    private static void TransitionTo(ref WalkAnimator walk, WalkAnimator.State newState, bool reverse)
    {
        if (walk.CurrentState == newState)
            return;

        walk.CurrentState = newState;
        walk.IsReverse = reverse;

        var frames = walk.CurrentAnimation.Frames;
        if (reverse)
        {
            walk.FrameIndex = (ushort)(frames.Length - 1);
            walk.Counter = frames.UnsafeAt(walk.FrameIndex).FrameDuration - 1;
        }
        else
        {
            walk.FrameIndex = 0;
            walk.Counter = 0;
        }
    }

    private static void AdvanceFrame(ref WalkAnimator walk)
    {
        var frames = walk.CurrentAnimation.Frames;
        int len = frames.Length;
        int frameIndex = walk.FrameIndex;
        int counter = walk.Counter;
        var state = walk.CurrentState;

        if (!walk.IsReverse)
        {
            counter++;
            while (counter >= frames.UnsafeAt(frameIndex).FrameDuration)
            {
                frameIndex++;
                if (frameIndex >= len)
                    break;
                counter = 0;
            }

            if (frameIndex >= len)
            {
                switch (state)
                {
                    case WalkAnimator.State.IdleLoop:
                    case WalkAnimator.State.LeftLoop:
                    case WalkAnimator.State.RightLoop:
                        frameIndex = 0;
                        counter = 0;
                        break;

                    case WalkAnimator.State.TransitionLeftLoop:
                        walk.CurrentState = WalkAnimator.State.LeftLoop;
                        frameIndex = 0;
                        counter = 0;
                        break;

                    case WalkAnimator.State.TransitionRightLoop:
                        walk.CurrentState = WalkAnimator.State.RightLoop;
                        frameIndex = 0;
                        counter = 0;
                        break;
                }
            }
        }
        else
        {
            counter--;
            while (counter < 0)
            {
                frameIndex--;
                if (frameIndex < 0)
                    break;
                counter = frames.UnsafeAt(frameIndex).FrameDuration - 1;
            }

            if (frameIndex < 0)
            {
                var dir = walk.CurrentDirection;

                if (state == WalkAnimator.State.TransitionLeftLoop && dir > 0)
                {
                    TransitionTo(ref walk, WalkAnimator.State.TransitionRightLoop, reverse: false);
                }
                else if (state == WalkAnimator.State.TransitionRightLoop && dir < 0)
                {
                    TransitionTo(ref walk, WalkAnimator.State.TransitionLeftLoop, reverse: false);
                }
                else
                {
                    walk.CurrentState = WalkAnimator.State.IdleLoop;
                    walk.IsReverse = false;
                    frameIndex = 0;
                    counter = 0;
                }
            }
        }

        walk.FrameIndex = (ushort)Math.Max(0, frameIndex);
        walk.Counter = Math.Max(0, counter);
    }
}

