using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed class ParticleManager
{
    private const int Capacity = 0xFFFF;
    private const int CurveCapacity = 0xFF;
    private const int SampleCount = 64;

    private static readonly float[] curves = new float[CurveCapacity * SampleCount];
    private static int curveCount;

    private int particleCount;

    #region Particle SoA Fields
    private readonly Vector2[]       pos          = new Vector2[Capacity];
    private readonly Vector2[]       vel          = new Vector2[Capacity];
    private readonly Vector2[]       acc          = new Vector2[Capacity];

    private readonly float[]         rot          = new float[Capacity];
    private readonly float[]         omega        = new float[Capacity];

    private readonly float[]         sizeX0       = new float[Capacity];
    private readonly float[]         sizeY0       = new float[Capacity];
    private readonly float[]         sizeX1       = new float[Capacity];
    private readonly float[]         sizeY1       = new float[Capacity];
    private readonly float[]         opacity0     = new float[Capacity];
    private readonly float[]         opacity1     = new float[Capacity];

    private readonly int[]           time         = new int[Capacity];
    private readonly int[]           duration     = new int[Capacity];

    private readonly Texture2D[]     texture      = new Texture2D[Capacity];
    private readonly Vector2[]       offset       = new Vector2[Capacity];
    private readonly Color[]         color        = new Color[Capacity];
    private readonly byte[]          layer        = new byte[Capacity];
    private readonly StgBlendState[] blend        = new StgBlendState[Capacity];

    private readonly byte[]          sizeXCurve   = new byte[Capacity];
    private readonly byte[]          sizeYCurve   = new byte[Capacity];
    private readonly byte[]          opacityCurve = new byte[Capacity];
    #endregion

    public int ParticleCount => particleCount;

    static ParticleManager()
    {
        AddCurve(Easing.Linear);
        AddCurve(Easing.InQuad);
        AddCurve(Easing.OutQuad);
        AddCurve(Easing.InOutQuad);
        AddCurve(Easing.OutInQuad);
        AddCurve(Easing.InCubic);
        AddCurve(Easing.OutCubic);
        AddCurve(Easing.InOutCubic);
        AddCurve(Easing.OutInCubic);
        AddCurve(Easing.SmoothStep);
    }

    private static int AddCurve(Func<float, float> curve)
    {
        if (curveCount == CurveCapacity) return -1;

        int baseIndex = curveCount * SampleCount;
        for (int i = 0; i < SampleCount; i++)
            curves[baseIndex + i] = curve(i / (float)(SampleCount - 1));

        return curveCount++;
    }

    public void Emit(
        Vector2 pos, Vector2 vel, Vector2 acc, float rot, float omega,
        float sizeX0, float sizeY0, float sizeX1, float sizeY1, float opacity0, float opacity1,
        Texture2D texture, Vector2 offset, Color color, byte layer, StgBlendState blend,
        int duration, byte sizeXCurve, byte sizeYCurve, byte opacityCurve)
    {
        int i = particleCount;
        if (i >= Capacity) return;

        this.pos.UnsafeAt(i)          = pos;
        this.vel.UnsafeAt(i)          = vel;
        this.acc.UnsafeAt(i)          = acc;
        this.rot.UnsafeAt(i)          = rot;
        this.omega.UnsafeAt(i)        = omega;
        this.sizeX0.UnsafeAt(i)       = sizeX0;
        this.sizeY0.UnsafeAt(i)       = sizeY0;
        this.sizeX1.UnsafeAt(i)       = sizeX1;
        this.sizeY1.UnsafeAt(i)       = sizeY1;
        this.opacity0.UnsafeAt(i)     = opacity0;
        this.opacity1.UnsafeAt(i)     = opacity1;
        this.duration.UnsafeAt(i)     = duration;
        this.texture.UnsafeAt(i)      = texture;
        this.offset.UnsafeAt(i)       = offset;
        this.color.UnsafeAt(i)        = color;
        this.layer.UnsafeAt(i)        = layer;
        this.blend.UnsafeAt(i)        = blend;
        this.sizeXCurve.UnsafeAt(i)   = sizeXCurve;
        this.sizeYCurve.UnsafeAt(i)   = sizeYCurve;
        this.opacityCurve.UnsafeAt(i) = opacityCurve;
        time.UnsafeAt(i)              = 0;

        particleCount++;
    }

    public unsafe void Update()
    {
        fixed (Vector2* pPos = pos, pVel = vel, pAcc = acc)
        fixed (float* pRot = rot, pOmega = omega)
        fixed (int* pTime = time, pDuration = duration)
        {
            int count = particleCount;
            int i = 0;

            while (i < count)
            {
                pTime[i]++;

                if (pTime[i] >= pDuration[i])
                {
                    count--;
                    if (i != count)
                        SwapCopy(count, i);
                    continue;
                }

                Vector2* v = pVel + i;
                Vector2* a = pAcc + i;
                Vector2* p = pPos + i;

                v->X += a->X;
                v->Y += a->Y;
                p->X += v->X;
                p->Y += v->Y;

                pRot[i] += pOmega[i];
                i++;
            }

            particleCount = count;
        }
    }

    private void SwapCopy(int src, int dst)
    {
        pos.UnsafeAt(dst)          = pos.UnsafeAt(src);
        vel.UnsafeAt(dst)          = vel.UnsafeAt(src);
        acc.UnsafeAt(dst)          = acc.UnsafeAt(src);
        rot.UnsafeAt(dst)          = rot.UnsafeAt(src);
        omega.UnsafeAt(dst)        = omega.UnsafeAt(src);
        sizeX0.UnsafeAt(dst)       = sizeX0.UnsafeAt(src);
        sizeY0.UnsafeAt(dst)       = sizeY0.UnsafeAt(src);
        sizeX1.UnsafeAt(dst)       = sizeX1.UnsafeAt(src);
        sizeY1.UnsafeAt(dst)       = sizeY1.UnsafeAt(src);
        opacity0.UnsafeAt(dst)     = opacity0.UnsafeAt(src);
        opacity1.UnsafeAt(dst)     = opacity1.UnsafeAt(src);
        time.UnsafeAt(dst)         = time.UnsafeAt(src);
        duration.UnsafeAt(dst)     = duration.UnsafeAt(src);
        texture.UnsafeAt(dst)      = texture.UnsafeAt(src);
        offset.UnsafeAt(dst)       = offset.UnsafeAt(src);
        color.UnsafeAt(dst)        = color.UnsafeAt(src);
        layer.UnsafeAt(dst)        = layer.UnsafeAt(src);
        blend.UnsafeAt(dst)        = blend.UnsafeAt(src);
        sizeXCurve.UnsafeAt(dst)   = sizeXCurve.UnsafeAt(src);
        sizeYCurve.UnsafeAt(dst)   = sizeYCurve.UnsafeAt(src);
        opacityCurve.UnsafeAt(dst) = opacityCurve.UnsafeAt(src);
    }

    public unsafe void Draw(StgBatch batch)
    {
        if (particleCount == 0) return;

        fixed (float* pCurves = curves)
        fixed (float* pSizeX0 = sizeX0, pSizeY0 = sizeY0)
        fixed (float* pSizeX1 = sizeX1, pSizeY1 = sizeY1)
        fixed (float* pOpacity0 = opacity0, pOpacity1 = opacity1)
        fixed (float* pRot = rot)
        fixed (int* pTime = time, pDuration = duration)
        fixed (Vector2* pPos = pos, pOffset = offset)
        fixed (Color* pColor = color)
        fixed (byte* pLayer = layer)
        fixed (byte* pSizeXCurve = sizeXCurve, pSizeYCurve = sizeYCurve, pOpacityCurve = opacityCurve)
        {
            int count = particleCount;

            for (int i = 0; i < count; i++)
            {
                int t = pTime[i];
                int d = pDuration[i];
                int sampleIndex = t * (SampleCount - 1) / d;

                float* curveX = pCurves + pSizeXCurve[i] * SampleCount + sampleIndex;
                float* curveY = pCurves + pSizeYCurve[i] * SampleCount + sampleIndex;
                float* curveO = pCurves + pOpacityCurve[i] * SampleCount + sampleIndex;

                float sizeX   = pSizeX0[i]   + (pSizeX1[i]   - pSizeX0[i])   * (*curveX);
                float sizeY   = pSizeY0[i]   + (pSizeY1[i]   - pSizeY0[i])   * (*curveY);
                float opacity = pOpacity0[i] + (pOpacity1[i] - pOpacity0[i]) * (*curveO);

                Color col = pColor[i];
                col.A = (byte)(col.A * opacity);

                batch.Draw(
                    texture[i],
                    pPos[i],
                    null,
                    col,
                    pRot[i],
                    pOffset[i],
                    new Vector2(sizeX, sizeY),
                    SpriteEffects.None,
                    pLayer[i],
                    blend[i]
                );
            }
        }
    }
}
