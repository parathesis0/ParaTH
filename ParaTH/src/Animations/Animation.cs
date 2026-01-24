using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public enum PlayType
{
    None, Hold, Loop, PingPong
}

public sealed class Animation
{
    private List<AnimationFrame> frames;
    private int frameIndex;
    private int tickCount;
    public bool IsPlaying { get; set; }
    public bool IsReverse { get; set; }
    public PlayType PlayType { get; set; }

    public AnimationFrame CurrentFrame
    {
        get
        {
            if (frames.Count == 0) return default;
            int index = Math.Clamp(frameIndex, 0, frames.Count - 1);
            return frames[index];
        }
    }

    public Animation(List<AnimationFrame> frames)
    {
        this.frames = frames;
    }

    public void AddFrame(AnimationFrame frame)
    {
        frames.Add(frame);
    }

    public void Play(bool fromStart = true)
    {
        IsPlaying = true;
        if (fromStart)
            Reset();
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
            frameIndex = frames.Count - 1;
            tickCount = frames[frameIndex].Duration;
        }
    }

    private bool IsAtBoundary()
    {
        return !IsReverse ?
            frameIndex == frames.Count : frameIndex == -1;
    }

    private void HandleBoundary()
    {
        if (!IsPlaying) return;

        switch (PlayType)
        {
            case PlayType.None:
                IsPlaying = false;
                Reset();
                break;
            case PlayType.Hold:
                IsPlaying = false;
                frameIndex = !IsReverse ? frames.Count - 1 : 0;
                break;
            case PlayType.Loop:
                Reset();
                break;
            case PlayType.PingPong:
                IsReverse = !IsReverse;
                if (!IsReverse)
                {
                    frameIndex = 1;
                    tickCount = 0;
                }
                else
                {
                    frameIndex = frames.Count - 2;
                    tickCount = frames[frameIndex].Duration;
                }
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void Update()
    {
        if (!IsPlaying) return;

        if (!IsReverse)
        {
            tickCount++;
            while (tickCount >= frames[frameIndex].Duration)
            {
                frameIndex++;
                if (frameIndex == frames.Count)
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
                if (frameIndex == -1)
                    break;

                tickCount = frames[frameIndex].Duration;
            }
        }

        if (IsAtBoundary())
            HandleBoundary();
    }
}

public readonly record struct AnimationFrame
{
    public readonly Texture2D Texture;
    public readonly Rectangle SourceRect;
    public readonly Vector2 Offset;
    public readonly int Duration;

    public AnimationFrame(Texture2D texture, Rectangle sourceRect, Vector2 offset, int duration)
    {
        Texture = texture;
        SourceRect = sourceRect;
        Offset = offset;
        Duration = duration;
    }
}
