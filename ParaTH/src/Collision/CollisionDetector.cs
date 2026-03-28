using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace ParaTH;

public static class CollisionDetector
{
    // really (early)optimized SAT
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(ObbRect rectA, Vector2 posA, ObbRect rectB, Vector2 posB)
    {
        float dx = posB.X - posA.X;
        float dy = posB.Y - posA.Y;

        // we don't use sincos here because it's slow as fuck on intel
        // 7x slower than sin and cos in benchmarks
        var sinA = MathF.Sin(rectA.Rotation);
        var cosA = MathF.Cos(rectA.Rotation);

        float tx = dx * cosA + dy * sinA;
        float ty = dy * cosA - dx * sinA;

        float deltaTheta = rectB.Rotation - rectA.Rotation;
        var sinB = MathF.Sin(deltaTheta);
        var cosB = MathF.Cos(deltaTheta);

        float absCos = MathF.Abs(cosB);
        float absSin = MathF.Abs(sinB);

        float aX = rectA.HalfSize.X, aY = rectA.HalfSize.Y;
        float bX = rectB.HalfSize.X, bY = rectB.HalfSize.Y;

        if (MathF.Abs(tx) >= aX + bX * absCos + bY * absSin)
            return false;

        if (MathF.Abs(ty) >= aY + bX * absSin + bY * absCos)
            return false;

        if (MathF.Abs(tx * cosB + ty * sinB) >= bX + aX * absCos + aY * absSin)
            return false;

        return MathF.Abs(ty * cosB - tx * sinB) < bY + aX * absSin + aY * absCos;
    }

    // transform circle to obb's
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(ObbRect rect, Vector2 posA, Circle circle, Vector2 posB)
    {
        var sinA = MathF.Sin(rect.Rotation);
        var cosA = MathF.Cos(rect.Rotation);

        var dx = posB.X - posA.X;
        var dy = posB.Y - posA.Y;

        var dw = Math.Max(0, MathF.Abs(dx *  cosA + dy * sinA) - rect.HalfSize.X);
        var dh = Math.Max(0, MathF.Abs(dx * -sinA + dy * cosA) - rect.HalfSize.Y);

        return dw * dw + dh * dh <= circle.Radius * circle.Radius;
    }

    // gemini wrote this approximation
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(ObbRect rect, Vector2 posA, Ellipse ellipse, Vector2 posB)
    {
        float dx = posA.X - posB.X;
        float dy = posA.Y - posB.Y;

        float cosE = MathF.Cos(ellipse.Rotation);
        float sinE = MathF.Sin(ellipse.Rotation);

        float localX = MathF.Abs(dx * cosE + dy * sinE);
        float localY = MathF.Abs(dy * cosE - dx * sinE);

        float deltaTheta = rect.Rotation - ellipse.Rotation;
        float cosD = MathF.Abs(MathF.Cos(deltaTheta));
        float sinD = MathF.Abs(MathF.Sin(deltaTheta));

        float projX = rect.HalfSize.X * cosD + rect.HalfSize.Y * sinD;
        float projY = rect.HalfSize.X * sinD + rect.HalfSize.Y * cosD;

        float extX = ellipse.HalfSize.X + projX;
        float extY = ellipse.HalfSize.Y + projY;

        float x2 = localX * localX;
        float y2 = localY * localY;
        float a2 = extX * extX;
        float b2 = extY * extY;

        return (x2 * b2) + (y2 * a2) <= (a2 * b2);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(Circle circle, Vector2 posA, ObbRect rect, Vector2 posB)
    {
        return Intersects(rect, posB, circle, posA);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(Circle circleA, Vector2 posA, Circle circleB, Vector2 posB)
    {
        var dx = posA.X - posB.X;
        var dy = posA.Y - posB.Y;
        var dist = circleA.Radius + circleB.Radius;
        return dx * dx + dy * dy <= dist * dist;
    }

    // gemini wrote this approximation
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(Circle circle, Vector2 posA, Ellipse ellipse, Vector2 posB)
    {
        float dx = posA.X - posB.X;
        float dy = posA.Y - posB.Y;

        float cosE = MathF.Cos(ellipse.Rotation);
        float sinE = MathF.Sin(ellipse.Rotation);

        float localX = MathF.Abs(dx * cosE + dy * sinE);
        float localY = MathF.Abs(dy * cosE - dx * sinE);

        float extX = ellipse.HalfSize.X + circle.Radius;
        float extY = ellipse.HalfSize.Y + circle.Radius;

        // (x/a)^2 + (y/b)^2 <= 1
        // and therefore x^2 * b^2 + y^2 * a^2 <= a^2 * b^2
        float x2 = localX * localX;
        float y2 = localY * localY;
        float a2 = extX * extX;
        float b2 = extY * extY;

        return (x2 * b2) + (y2 * a2) <= (a2 * b2);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(Ellipse ellipse, Vector2 posA, ObbRect rect, Vector2 posB)
    {
        return Intersects(rect, posB, ellipse, posA);
    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(Ellipse ellipse, Vector2 posA, Circle Circle, Vector2 posB)
    {
        return Intersects(Circle, posB, ellipse, posA);
    }

    // gemini wrote this approximation
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects(Ellipse ellipseA, Vector2 posA, Ellipse ellipseB, Vector2 posB)
    {
        float dx = posB.X - posA.X;
        float dy = posB.Y - posA.Y;

        float cosA_rot = MathF.Cos(ellipseA.Rotation);
        float sinA_rot = MathF.Sin(ellipseA.Rotation);

        float localX = MathF.Abs(dx * cosA_rot + dy * sinA_rot);
        float localY = MathF.Abs(dy * cosA_rot - dx * sinA_rot);

        float deltaTheta = ellipseB.Rotation - ellipseA.Rotation;
        float cosD = MathF.Cos(deltaTheta);
        float sinD = MathF.Sin(deltaTheta);

        float axCos = ellipseB.HalfSize.X * cosD;
        float bxSin = ellipseB.HalfSize.Y * sinD;
        float projX = MathF.Sqrt(axCos * axCos + bxSin * bxSin);

        float axSin = ellipseB.HalfSize.X * sinD;
        float bxCos = ellipseB.HalfSize.Y * cosD;
        float projY = MathF.Sqrt(axSin * axSin + bxCos * bxCos);

        float extX = ellipseA.HalfSize.X + projX;
        float extY = ellipseA.HalfSize.Y + projY;

        float x2 = localX * localX;
        float y2 = localY * localY;
        float a2 = extX * extX;
        float b2 = extY * extY;

        return (x2 * b2) + (y2 * a2) <= (a2 * b2);
    }
}
