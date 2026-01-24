using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed class ParticleManager
{
    private const int Capacity = 0xFFFF;
    private const int CurveCapacity = 0xFF;
    private const int SampleCount = 64;

    private static float[] curves = new float[CurveCapacity * SampleCount];
    private static int curveCount;

    private int particleCount;

    #region Particle SoA Fields
    private readonly float[]     x            = new float[Capacity];
    private readonly float[]     y            = new float[Capacity];

    private readonly float[]     vx           = new float[Capacity];
    private readonly float[]     vy           = new float[Capacity];

    private readonly float[]     ax           = new float[Capacity];
    private readonly float[]     ay           = new float[Capacity];

    private readonly float[]     rot          = new float[Capacity];
    private readonly float[]     omega        = new float[Capacity];

    private readonly float[]     sizeX0       = new float[Capacity];
    private readonly float[]     sizeY0       = new float[Capacity];
    private readonly float[]     sizeX1       = new float[Capacity];
    private readonly float[]     sizeY1       = new float[Capacity];
    private readonly float[]     opacity0     = new float[Capacity];
    private readonly float[]     opacity1     = new float[Capacity];

    private readonly int[]       time         = new int[Capacity];
    private readonly int[]       duration     = new int[Capacity];

    private readonly Texture2D[] texture      = new Texture2D[Capacity];
    private readonly float[]     offsetX      = new float[Capacity];
    private readonly float[]     offsetY      = new float[Capacity];
    private readonly Color[]     color        = new Color[Capacity];
    private readonly byte[]      layer        = new byte[Capacity];

    private readonly byte[]      sizeXCurve   = new byte[Capacity];
    private readonly byte[]      sizeYCurve   = new byte[Capacity];
    private readonly byte[]      opacityCurve = new byte[Capacity];
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
        float x, float y, float vx, float vy, float ax, float ay, float rot, float omega,
        float sizeX0, float sizeY0, float sizeX1, float sizeY1, float opacity0, float opacity1,
        Texture2D texture, float offsetX, float offsetY, Color color, byte layer,
        int duration, byte sizeXCurve, byte sizeYCurve, byte opacityCurve)
    {
        if (particleCount >= Capacity) return;

        this.x[particleCount]            = x;
        this.y[particleCount]            = y;
        this.vx[particleCount]           = vx;
        this.vy[particleCount]           = vy;
        this.ax[particleCount]           = ax;
        this.ay[particleCount]           = ay;
        this.rot[particleCount]          = rot;
        this.omega[particleCount]        = omega;
        this.sizeX0[particleCount]       = sizeX0;
        this.sizeY0[particleCount]       = sizeY0;
        this.sizeX1[particleCount]       = sizeX1;
        this.sizeY1[particleCount]       = sizeY1;
        this.opacity0[particleCount]     = opacity0;
        this.opacity1[particleCount]     = opacity1;
        this.duration[particleCount]     = duration;
        this.texture[particleCount]      = texture;
        this.offsetX[particleCount]      = offsetX;
        this.offsetY[particleCount]      = offsetY;
        this.color[particleCount]        = color;
        this.layer[particleCount]        = layer;
        this.sizeXCurve[particleCount]   = sizeXCurve;
        this.sizeYCurve[particleCount]   = sizeYCurve;
        this.opacityCurve[particleCount] = opacityCurve;

        time[particleCount] = 0;

        particleCount++;
    }

    public void Update()
    {
        int i = 0;

        while (i < particleCount)
        {
            time[i]++;
            if (time[i] >= duration[i])
            {
                particleCount--;
                if (i != particleCount)
                    SwapCopy(particleCount, i);
                continue;
            }

            vx[i] += ax[i];
            vy[i] += ay[i];

            x[i] += vx[i];
            y[i] += vy[i];

            rot[i] += omega[i];

            i++;
        }
    }

    private void SwapCopy(int src, int dst)
    {
        x[dst]            = x[src];
        y[dst]            = y[src];
        vx[dst]           = vx[src];
        vy[dst]           = vy[src];
        ax[dst]           = ax[src];
        ay[dst]           = ay[src];
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
        offsetX[dst]      = offsetX[src];
        offsetY[dst]      = offsetY[src];
        color[dst]        = color[src];
        layer[dst]        = layer[src];
        sizeXCurve[dst]   = sizeXCurve[src];
        sizeYCurve[dst]   = sizeYCurve[src];
        opacityCurve[dst] = opacityCurve[src];
    }

    public void Draw(SuperBatch batch)
    {
        for (int i = 0; i < particleCount; i++)
        {
            int sampleIndex = time[i] * (SampleCount - 1) / duration[i];
            float sizeX   = MathHelper.Lerp(sizeX0[i], sizeX1[i], curves[sizeXCurve[i] * SampleCount + sampleIndex]);
            float sizeY   = MathHelper.Lerp(sizeY0[i], sizeY1[i], curves[sizeYCurve[i] * SampleCount + sampleIndex]);
            float opacity = MathHelper.Lerp(opacity0[i], opacity1[i], curves[opacityCurve[i] * SampleCount + sampleIndex]);

            Color col = color[i];
            float finalAlpha = (col.A / 255f) * opacity;
            col.A = (byte)(finalAlpha * 255);

            batch.Draw(
                texture[i],
                new Vector2(x[i], y[i]),
                null,
                col,
                rot[i],
                new Vector2(offsetX[i], offsetY[i]),
                new Vector2(sizeX, sizeY),
                SpriteEffects.None,
                layer[i]
            );
        }
    }
}
