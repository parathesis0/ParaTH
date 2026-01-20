using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public class ParaSpriteEffect : Effect
{
    private EffectParameter matrixParam = null!;

    public ParaSpriteEffect(GraphicsDevice device, byte[] effectCode) : base(device, effectCode)
    {
        CacheParameters();
    }

    public ParaSpriteEffect(Effect cloneSource) : base(cloneSource)
    {
        CacheParameters();
    }

    private void CacheParameters()
    {
        matrixParam = Parameters["MatrixTransform"];
    }

    public void SetMatrix(Matrix matrix)
    {
        matrixParam.SetValue(matrix);
    }
}
