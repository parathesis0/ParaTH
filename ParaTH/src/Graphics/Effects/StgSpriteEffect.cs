using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public class StgSpriteEffect : Effect
{
    private EffectParameter matrixParam = null!;

    public StgSpriteEffect(GraphicsDevice device, byte[] effectCode) : base(device, effectCode)
    {
        CacheParameters();
    }

    public StgSpriteEffect(Effect cloneSource) : base(cloneSource)
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
