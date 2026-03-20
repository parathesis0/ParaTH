namespace ParaTH;

public delegate float EasingFunction(float t);
public static class Easing
{
    public static float Linear(float t) => t;

    public static float InQuad(float t) => t * t;
    public static float OutQuad(float t) => 1 - (1 - t) * (1 - t);
    public static float InOutQuad(float t)
    {
        return t < 0.5f
            ? 2 * t * t
            : 1 - (-2 * t + 2) * (-2 * t + 2) / 2;
    }
    public static float OutInQuad(float t)
    {
        return t < 0.5f
            ? (1 - (1 - 2 * t) * (1 - 2 * t)) / 2
            : (2 * (t - 0.5f) * (t - 0.5f)) + 0.5f;
    }

    public static float InCubic(float t)
    {
        return t * t * t;
    }
    public static float OutCubic(float t)
    {
        float f = t - 1;
        return f * f * f + 1;
    }
    public static float InOutCubic(float t)
    {
        return t < 0.5f
            ? 4 * t * t * t
            : 1 - (-2 * t + 2) * (-2 * t + 2) * (-2 * t + 2) / 2;
    }
    public static float OutInCubic(float t)
    {
        return t < 0.5f
            ? (1 - (1 - 2 * t) * (1 - 2 * t) * (1 - 2 * t)) / 2
            : (2 * (t - 0.5f) * (t - 0.5f) * (t - 0.5f)) + 0.5f;
    }

    public static float InQuart(float t)
    {
        return t * t * t * t;
    }
    public static float OutQuart(float t)
    {
        float f = 1 - t;
        return 1 - f * f * f * f;
    }
    public static float InOutQuart(float t)
    {
        return t < 0.5f
            ? 8 * t * t * t * t
            : 1 - ((-2 * t + 2) * (-2 * t + 2) * (-2 * t + 2) * (-2 * t + 2)) / 2;
    }
    public static float OutInQuart(float t)
    {
        return t < 0.5f
            ? (1 - (1 - 2 * t) * (1 - 2 * t) * (1 - 2 * t) * (1 - 2 * t)) / 2
            : ((2 * (t - 0.5f)) * (2 * (t - 0.5f)) * (2 * (t - 0.5f)) * (2 * (t - 0.5f))) / 2 + 0.5f;
    }

    public static float InQuint(float t) => t * t * t * t * t;
    public static float OutQuint(float t)
    {
        float f = t - 1;
        return f * f * f * f * f + 1;
    }
    public static float InOutQuint(float t)
    {
        return t < 0.5f
            ? 16 * t * t * t * t * t
            : 1 - (-2 * t + 2) * (-2 * t + 2) * (-2 * t + 2) * (-2 * t + 2) * (-2 * t + 2) / 2;
    }
    public static float OutInQuint(float t)
    {
        return t < 0.5f
            ? (1 - (1 - 2 * t) * (1 - 2 * t) * (1 - 2 * t) * (1 - 2 * t) * (1 - 2 * t)) / 2
            : (2 * (t - 0.5f) * (t - 0.5f) * (t - 0.5f) * (t - 0.5f) * (t - 0.5f)) + 0.5f;
    }

    public static float InExpo(float t)
    {
        return t == 0 ? 0 : MathF.Pow(2, 10 * t - 10);
    }
    public static float OutExpo(float t)
    {
        return t == 1 ? 1 : 1 - MathF.Pow(2, -10 * t);
    }
    public static float InOutExpo(float t)
    {
        if (t == 0) return 0;
        if (t == 1) return 1;
        return t < 0.5f
            ? MathF.Pow(2, 20 * t - 10) / 2
            : (2 - MathF.Pow(2, -20 * t + 10)) / 2;
    }
    public static float OutInExpo(float t)
    {
        if (t == 0.5f) return 0.5f;
        return t < 0.5f
            ? (1 - MathF.Pow(2, -20 * t)) / 2
            : (MathF.Pow(2, 20 * (t - 1)) + 1) / 2;
    }

    public static float InInverse(float t)
    {
        return t == 0 ? 0 : 1 - (1 / t);
    }
    public static float OutInverse(float t)
    {
        return t == 1 ? 1 : 1 / (1 - t);
    }
    public static float InOutInverse(float t)
    {
        if (t == 0) return 0;
        if (t == 1) return 1;

        if (t < 0.5f)
            return (1 - (1 / (2 * t))) / 2;
        else
            return (1 + (1 / (2 * (1 - t)))) / 2;
    }
    public static float OutInInverse(float t)
    {
        if (t == 0.5f) return 0.5f;

        if (t < 0.5f)
            return t / (1 - 2 * t);
        else
            return (t - 0.5f) / (2 * t - 1) + 0.5f;
    }

    public static float InCirc(float t) => -(MathF.Sqrt(1 - t * t) - 1);
    public static float OutCirc(float t) => 1 - InCirc(1 - t);
    public static float InOutCirc(float t)
    {
        if (t < 0.5) return InCirc(t * 2) / 2;
        return 1 - InCirc((1 - t) * 2) / 2;
    }
    public static float InElastic(float t) => 1 - OutElastic(1 - t);
    public static float OutElastic(float t)
    {
        float p = 0.3f;
        return MathF.Pow(2, -10 * t) * MathF.Sin((t - p / 4) * (2 * MathF.PI) / p) + 1;
    }
    public static float InOutElastic(float t)
    {
        if (t < 0.5) return InElastic(t * 2) / 2;
        return 1 - InElastic((1 - t) * 2) / 2;
    }
    public static float InBack(float t)
    {
        float s = 1.70158f;
        return t * t * ((s + 1) * t - s);
    }
    public static float OutBack(float t) => 1 - InBack(1 - t);
    public static float InOutBack(float t)
    {
        if (t < 0.5) return InBack(t * 2) / 2;
        return 1 - InBack((1 - t) * 2) / 2;
    }

    public static float InBounce(float t) => 1 - OutBounce(1 - t);
    public static float OutBounce(float t)
    {
        float div = 2.75f;
        float mult = 7.5625f;

        if (t < 1 / div)
        {
            return mult * t * t;
        }
        else if (t < 2 / div)
        {
            t -= 1.5f / div;
            return mult * t * t + 0.75f;
        }
        else if (t < 2.5 / div)
        {
            t -= 2.25f / div;
            return mult * t * t + 0.9375f;
        }
        else
        {
            t -= 2.625f / div;
            return mult * t * t + 0.984375f;
        }
    }
    public static float InOutBounce(float t)
    {
        if (t < 0.5) return InBounce(t * 2) / 2;
        return 1 - InBounce((1 - t) * 2) / 2;
    }

    public static float SmoothStep(float t)
    {
        return t * t * (3 - 2 * t);
    }
    public static float QuintSmoothStep(float t)
    {
        return t * t * t * (6 * t * t - 15 * t + 10);
    }
}

