using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

// todo: refactor once there's more renderers
public sealed class RenderSystem(World world, StgBatch batch)
{
    private QueryDescriptor query = new QueryDescriptor()
        .WithAll<Transform>()
        .WithAny<SpriteRenderer>(); // all renderers, there will be more in the future eg. AnimationRenderer, MovementRenderer, CurvyLaserRenderer etc.

    public void Update()
    {
        var q = world.GetOrCreateQuery(query);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            if (archetype.Has<SpawnAnimation>())
            {
                foreach (ref var chunk in archetype.GetChunksSpan())
                {
                    chunk.GetFilledComponentSpan<Transform, SpriteRenderer, SpawnAnimation>(
                        out var transforms, out var renderers, out var spawnAnims);

                    for (int i = 0; i < chunk.EntityCount; i++)
                    {
                        var spawnAnim = spawnAnims.UnsafeAt(i);
                        var transform = transforms.UnsafeAt(i);
                        var renderer = renderers.UnsafeAt(i);

                        var sprite = renderer.Sprite;
                        var color = renderer.Color;
                        var scaleMultiplier = 1f;

                        if (spawnAnim.Counter < spawnAnim.Duration)
                        {
                            sprite = spawnAnim.Sprite;

                            var t = (float)(spawnAnim.Counter + 1) / spawnAnim.Duration;
                            t = Easing.Evaluate(spawnAnim.Type, t);
                            scaleMultiplier = (float)Half.Lerp(spawnAnim.StartScaleMultiplier, (Half)1, (Half)t);
                            var alphaMultiplier = (float)Half.Lerp(spawnAnim.StartAlphaMultiplier, (Half)1, (Half)t);
                            color.A = (byte)(color.A * alphaMultiplier);
                        }

                        batch.Draw(
                            sprite.Texture,
                            transform.Position,
                            sprite.SourceRect,
                            color,
                            transform.Rotation,
                            renderer.Sprite.Anchor,
                            transform.Scale * scaleMultiplier,
                            SpriteEffects.None,
                            renderer.Layer,
                            renderer.BlendState);
                    }
                }
            }
            else if (archetype.Has<SpriteRenderer>())
            {
                foreach (ref var chunk in archetype.GetChunksSpan())
                {
                    chunk.GetFilledComponentSpan<Transform, SpriteRenderer>(
                        out var transforms, out var renderers);

                    for (int i = 0; i < chunk.EntityCount; i++)
                    {
                        var transform = transforms.UnsafeAt(i);
                        var renderer = renderers.UnsafeAt(i);

                        batch.Draw(
                            renderer.Sprite.Texture,
                            transform.Position,
                            renderer.Sprite.SourceRect,
                            renderer.Color,
                            transform.Rotation,
                            renderer.Sprite.Anchor,
                            transform.Scale,
                            SpriteEffects.None,
                            renderer.Layer,
                            renderer.BlendState);
                    }
                }
            }
        }
    }
}
