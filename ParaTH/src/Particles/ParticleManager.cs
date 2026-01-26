using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ref T At<T>(T[] array, int index) =>
        ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(array), index);

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

        At(this.pos,          i) = pos;
        At(this.vel,          i) = vel;
        At(this.acc,          i) = acc;
        At(this.rot,          i) = rot;
        At(this.omega,        i) = omega;
        At(this.sizeX0,       i) = sizeX0;
        At(this.sizeY0,       i) = sizeY0;
        At(this.sizeX1,       i) = sizeX1;
        At(this.sizeY1,       i) = sizeY1;
        At(this.opacity0,     i) = opacity0;
        At(this.opacity1,     i) = opacity1;
        At(this.duration,     i) = duration;
        At(this.texture,      i) = texture;
        At(this.offset,       i) = offset;
        At(this.color,        i) = color;
        At(this.layer,        i) = layer;
        At(this.blend,        i) = blend;
        At(this.sizeXCurve,   i) = sizeXCurve;
        At(this.sizeYCurve,   i) = sizeYCurve;
        At(this.opacityCurve, i) = opacityCurve;

        time[i] = 0;

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
        pos[dst]          = pos[src];
        vel[dst]          = vel[src];
        acc[dst]          = acc[src];
        rot[dst]          = rot[src];
        omega[dst]        = omega[src];
        sizeX0[dst]       = sizeX0[src];
        sizeY0[dst]       = sizeY0[src];
        sizeX1[dst]       = sizeX1[src];
        sizeY1[dst]       = sizeY1[src];
        opacity0[dst]     = opacity0[src];
        opacity1[dst]     = opacity1[src];
        time[dst]         = time[src];
        duration[dst]     = duration[src];
        texture[dst]      = texture[src];
        offset[dst]       = offset[src];
        color[dst]        = color[src];
        layer[dst]        = layer[src];
        blend[dst]        = blend[src];
        sizeXCurve[dst]   = sizeXCurve[src];
        sizeYCurve[dst]   = sizeYCurve[src];
        opacityCurve[dst] = opacityCurve[src];
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
