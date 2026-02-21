using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

using static AnimationAsset;

public sealed class TestAnimationPlayer
{
    private AnimationAsset? asset;
    private int frameIndex;
    private int tickCount;

    public bool IsPlaying { get; private set; }
    public bool IsReverse { get; private set; }

    public AnimationAsset? CurrentAsset => asset;

    public Frame CurrentFrame
    {
        get
        {
            if (asset is null || asset.Frames.Count == 0) return default;
            int index = Math.Clamp(frameIndex, 0, asset.Frames.Count - 1);
            return asset.Frames[index];
        }
    }

    public void Play(AnimationAsset animation, bool fromStart = true)
    {
        if (asset != animation)
        {
            asset = animation;
            IsReverse = false;
            Reset();
        }
        else if (fromStart)
        {
            IsReverse = false;
            Reset();
        }

        IsPlaying = true;
    }

    public void Stop()
    {
        IsPlaying = false;
    }

    private void Reset()
    {
        if (!IsReverse)
        {
            frameIndex = 0;
            tickCount = 0;
        }
        else
        {
            frameIndex = asset!.Frames.Count - 1;
            tickCount = (int)asset.Frames[frameIndex].FrameDuration;
        }
    }

    private bool IsAtBoundary()
    {
        return !IsReverse
            ? frameIndex >= asset!.Frames.Count
            : frameIndex < 0;
    }

    private void HandleBoundary()
    {
        switch (asset!.Type)
        {
            case PlayType.Hold:
                IsPlaying = false;
                frameIndex = !IsReverse ? asset.Frames.Count - 1 : 0;
                break;

            case PlayType.Loop:
                Reset();
                break;

            case PlayType.PingPong:
                IsReverse = !IsReverse;
                if (!IsReverse)
                {
                    frameIndex = Math.Min(1, asset.Frames.Count - 1);
                    tickCount = 0;
                }
                else
                {
                    frameIndex = Math.Max(asset.Frames.Count - 2, 0);
                    tickCount = (int)asset.Frames[frameIndex].FrameDuration;
                }
                break;
        }
    }

    public void Update()
    {
        if (!IsPlaying || asset is null || asset.Frames.Count == 0)
            return;

        if (!IsReverse)
        {
            tickCount++;
            while (tickCount >= (int)asset.Frames[frameIndex].FrameDuration)
            {
                frameIndex++;
                if (frameIndex >= asset.Frames.Count)
                    break;
                tickCount = 0;
            }
        }
        else
        {
            tickCount--;
            while (tickCount <= 0)
            {
                frameIndex--;
                if (frameIndex < 0)
                    break;
                tickCount = (int)asset.Frames[frameIndex].FrameDuration;
            }
        }

        if (IsAtBoundary())
            HandleBoundary();
    }
}
