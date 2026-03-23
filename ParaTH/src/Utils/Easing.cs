using System.Runtime.CompilerServices;

namespace ParaTH;

public enum EaseType : byte
{
    Linear,
    InQuad,
    OutQuad,
    InOutQuad,
    OutInQuad,
    InCubic,
    OutCubic,
    InOutCubic,
    OutInCubic,
    InQuart,
    OutQuart,
    InOutQuart,
    OutInQuart,
    InQuint,
    OutQuint,
    InOutQuint,
    OutInQuint,
    InExpo,
    OutExpo,
    InOutExpo,
    OutInExpo,
    InInverse,
    OutInverse,
    InOutInverse,
    OutInInverse,
    InCirc,
    OutCirc,
    InOutCirc,
    InElastic,
    OutElastic,
    InOutElastic,
    InBack,
    OutBack,
    InOutBack,
    InBounce,
    OutBounce,
    InOutBounce,
    SmoothStep,
    QuintSmoothStep,
    Count, // sentry
}

public delegate float EasingFunction(float t);

public static class Easing
{
    private static readonly EasingFunction[] Functions =
    [
        Linear,
        InQuad,
        OutQuad,
        InOutQuad,
        OutInQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        OutInCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        OutInQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        OutInQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        OutInExpo,
        InInverse,
        OutInverse,
        InOutInverse,
        OutInInverse,
        InCirc,
        OutCirc,
        InOutCirc,
        InElastic,
        OutElastic,
        InOutElastic,
        InBack,
        OutBack,
        InOutBack,
        InBounce,
        OutBounce,
        InOutBounce,
        SmoothStep,
        QuintSmoothStep,
    ];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EasingFunction Get(EaseType type) => Functions.UnsafeAt((byte)type);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Evaluate(EaseType type, float t) => Functions.UnsafeAt((byte)type)(t);

    public static float Linear(float t) => t;

    public static float InQuad(float t) => t * t;
    public static float OutQuad(float t) => 1 - (1 - t) * (1 - t);
    public static float InOutQuad(float t) =>
        t < 0.5f ? 2 * t * t : 1 - (-2 * t + 2) * (-2 * t + 2) / 2;
    public static float OutInQuad(float t) =>
        t < 0.5f ? (1 - (1 - 2 * t) * (1 - 2 * t)) / 2 : (2 * (t - 0.5f) * (t - 0.5f)) + 0.5f;

    public static float InCubic(float t) => t * t * t;
    public static float OutCubic(float t) { float f = t - 1; return f * f * f + 1; }
    public static float InOutCubic(float t) =>
        t < 0.5f ? 4 * t * t * t : 1 - (-2 * t + 2) * (-2 * t + 2) * (-2 * t + 2) / 2;
    public static float OutInCubic(float t) =>
        t < 0.5f ? (1 - (1 - 2 * t) * (1 - 2 * t) * (1 - 2 * t)) / 2 : (2 * (t - 0.5f) * (t - 0.5f) * (t - 0.5f)) + 0.5f;

    public static float InQuart(float t) => t * t * t * t;
    public static float OutQuart(float t) { float f = 1 - t; return 1 - f * f * f * f; }
    public static float InOutQuart(float t) =>
        t < 0.5f ? 8 * t * t * t * t : 1 - ((-2 * t + 2) * (-2 * t + 2) * (-2 * t + 2) * (-2 * t + 2)) / 2;
    public static float OutInQuart(float t) =>
        t < 0.5f ? (1 - (1 - 2 * t) * (1 - 2 * t) * (1 - 2 * t) * (1 - 2 * t)) / 2
                 : ((2 * (t - 0.5f)) * (2 * (t - 0.5f)) * (2 * (t - 0.5f)) * (2 * (t - 0.5f))) / 2 + 0.5f;

    public static float InQuint(float t) => t * t * t * t * t;
    public static float OutQuint(float t) { float f = t - 1; return f * f * f * f * f + 1; }
    public static float InOutQuint(float t) =>
        t < 0.5f ? 16 * t * t * t * t * t : 1 - (-2 * t + 2) * (-2 * t + 2) * (-2 * t + 2) * (-2 * t + 2) * (-2 * t + 2) / 2;
    public static float OutInQuint(float t) =>
        t < 0.5f ? (1 - (1 - 2 * t) * (1 - 2 * t) * (1 - 2 * t) * (1 - 2 * t) * (1 - 2 * t)) / 2
                 : (2 * (t - 0.5f) * (t - 0.5f) * (t - 0.5f) * (t - 0.5f) * (t - 0.5f)) + 0.5f;

    public static float InExpo(float t) => t == 0 ? 0 : MathF.Pow(2, 10 * t - 10);
    public static float OutExpo(float t) => t == 1 ? 1 : 1 - MathF.Pow(2, -10 * t);
    public static float InOutExpo(float t)
    {
        if (t == 0) return 0;
        if (t == 1) return 1;
        return t < 0.5f ? MathF.Pow(2, 20 * t - 10) / 2 : (2 - MathF.Pow(2, -20 * t + 10)) / 2;
    }
    public static float OutInExpo(float t)
    {
        if (t == 0.5f) return 0.5f;
        return t < 0.5f ? (1 - MathF.Pow(2, -20 * t)) / 2 : (MathF.Pow(2, 20 * (t - 1)) + 1) / 2;
    }

    public static float InInverse(float t) => t == 0 ? 0 : 1 - (1 / t);
    public static float OutInverse(float t) => t == 1 ? 1 : 1 / (1 - t);
    public static float InOutInverse(float t)
    {
        if (t == 0) return 0;
        if (t == 1) return 1;
        return t < 0.5f ? (1 - (1 / (2 * t))) / 2 : (1 + (1 / (2 * (1 - t)))) / 2;
    }
    public static float OutInInverse(float t)
    {
        if (t == 0.5f) return 0.5f;
        return t < 0.5f ? t / (1 - 2 * t) : (t - 0.5f) / (2 * t - 1) + 0.5f;
    }

    public static float InCirc(float t) => -(MathF.Sqrt(1 - t * t) - 1);
    public static float OutCirc(float t) => 1 - InCirc(1 - t);
    public static float InOutCirc(float t) =>
        t < 0.5f ? InCirc(t * 2) / 2 : 1 - InCirc((1 - t) * 2) / 2;

    public static float InElastic(float t) => 1 - OutElastic(1 - t);
    public static float OutElastic(float t)
    {
        const float P = 0.3f;
        return MathF.Pow(2, -10 * t) * MathF.Sin((t - P / 4) * (2 * MathF.PI) / P) + 1;
    }
    public static float InOutElastic(float t) =>
        t < 0.5f ? InElastic(t * 2) / 2 : 1 - InElastic((1 - t) * 2) / 2;

    public static float InBack(float t) { const float S = 1.70158f; return t * t * ((S + 1) * t - S); }
    public static float OutBack(float t) => 1 - InBack(1 - t);
    public static float InOutBack(float t) =>
        t < 0.5f ? InBack(t * 2) / 2 : 1 - InBack((1 - t) * 2) / 2;

    public static float InBounce(float t) => 1 - OutBounce(1 - t);
    public static float OutBounce(float t)
    {
        const float Div = 2.75f, Mult = 7.5625f;
        if (t < 1 / Div) return Mult * t * t;
        if (t < 2 / Div) { t -= 1.5f / Div; return Mult * t * t + 0.75f; }
        if (t < 2.5f / Div) { t -= 2.25f / Div; return Mult * t * t + 0.9375f; }
        t -= 2.625f / Div; return Mult * t * t + 0.984375f;
    }
    public static float InOutBounce(float t) =>
        t < 0.5f ? InBounce(t * 2) / 2 : 1 - InBounce((1 - t) * 2) / 2;

    public static float SmoothStep(float t) => t * t * (3 - 2 * t);
    public static float QuintSmoothStep(float t) => t * t * t * (6 * t * t - 15 * t + 10);
}
