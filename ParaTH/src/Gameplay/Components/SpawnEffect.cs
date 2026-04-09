using Microsoft.Xna.Framework;

namespace ParaTH;

// 24 bytes
public struct SpawnEffect(SpriteAsset sprite, Vector2 startScale, Half startAlpha,
                          Half multiplier, byte duration, EaseType easeTypeX, EaseType easeTypeY)
{
    public SpriteAsset Sprite = sprite;             // 8
    public Vector2 StartScale = startScale;         // 4 + 4
    public Half StartAlpha = startAlpha;            // 2
    public Half VelocityMultiplier = multiplier;    // 2
    public EaseType TypeX = easeTypeX;              // 1
    public EaseType TypeY = easeTypeY;              // 1
    public byte Duration = duration;                // 1
    public byte Counter = 0;                        // 1
    public readonly bool IsPlaying => Counter < Duration;
}

