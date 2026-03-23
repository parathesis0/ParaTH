namespace ParaTH;

// 16 bytes, not including debug bytes
public struct SpawnAnimation(SpriteAsset sprite, Half startSizeMultiplier, Half startAlphaMultiplier,
                             Half velocityMultiplier, byte duration, EaseType easeType)
{
    public SpriteAsset Sprite = sprite;                            // 8
    public Half StartScaleMultiplier = startSizeMultiplier;        // 2
    public Half StartAlphaMultiplier = startAlphaMultiplier;       // 2
    public Half SpawningVelocityMultiplier = velocityMultiplier;   // 2
    public byte Duration = duration;                               // 1
    public EaseType Type = easeType;                               // 1

    public byte Counter = 0; // TODO: REMOVE THIS, PUT COUNTER IN LIFETIME
}

