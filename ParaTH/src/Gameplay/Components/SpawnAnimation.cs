namespace ParaTH;

// 16 bytes
public struct SpawnAnimation(SpriteAsset sprite, Half startSizeMultiplier, Half startAlphaMultiplier,
                             byte velocityFactor, byte duration, EaseType easeType)
{
    public SpriteAsset Sprite = sprite;                             // 8
    public Half StartScaleMultiplier = startSizeMultiplier;         // 2
    public Half StartAlphaMultiplier = startAlphaMultiplier;        // 2
    public byte SpawningVelocityMultiplierFixed = velocityFactor;   // 1 fixed point
    public byte Duration = duration;                                // 1
    public byte Counter = 0;                                        // 1
    public EaseType Type = easeType;                                // 1

    public const float FixedPointScale = 16f;
    public const float FixedPointInv = 1f / FixedPointScale;
    public readonly float SpawningVelocityMultiplier => SpawningVelocityMultiplierFixed * FixedPointInv;
}

