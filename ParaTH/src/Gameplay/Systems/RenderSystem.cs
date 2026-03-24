using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

// todos:
// refactor once there's more renderers, use modifier pattern maybe
// prob should extract color blendstate layer into a separate component, put Scale there too.
public sealed class RenderSystem(World world, StgBatch batch)
{
    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform>()
        .WithAny<SpriteRenderer, AnimationRenderer>(); // remember to add more renderers here

    // todo: unbearable, refactor
    public void Update()
    {
        var q = world.GetOrCreateQuery(descriptor);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            if (archetype.Has<SpawnAnimation>())
            {
                if (archetype.Has<SpriteRenderer>())
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
                                sprite.Anchor,
                                transform.Scale * scaleMultiplier,
                                SpriteEffects.None,
                                renderer.Layer,
                                renderer.BlendState);
                        }
                    }
                }
                else if (archetype.Has<AnimationRenderer>())
                {
                    foreach (ref var chunk in archetype.GetChunksSpan())
                    {
                        chunk.GetFilledComponentSpan<Transform, AnimationRenderer, SpawnAnimation>(
                            out var transforms, out var renderers, out var spawnAnims);

                        for (int i = 0; i < chunk.EntityCount; i++)
                        {
                            var spawnAnim = spawnAnims.UnsafeAt(i);
                            var transform = transforms.UnsafeAt(i);
                            var renderer = renderers.UnsafeAt(i);

                            var texture = renderer.Animation.Texture;
                            var rect = renderer.CurrentFrame.SourceRect;
                            var anchor = renderer.CurrentFrame.Anchor;
                            var color = renderer.Color;
                            var scaleMultiplier = 1f;

                            if (spawnAnim.Counter < spawnAnim.Duration)
                            {
                                texture = spawnAnim.Sprite.Texture;
                                rect = spawnAnim.Sprite.SourceRect;
                                anchor = spawnAnim.Sprite.Anchor;
                                var t = (float)(spawnAnim.Counter + 1) / spawnAnim.Duration;
                                t = Easing.Evaluate(spawnAnim.Type, t);
                                scaleMultiplier = (float)Half.Lerp(spawnAnim.StartScaleMultiplier, (Half)1, (Half)t);
                                var alphaMultiplier = (float)Half.Lerp(spawnAnim.StartAlphaMultiplier, (Half)1, (Half)t);
                                color.A = (byte)(color.A * alphaMultiplier);
                            }

                            batch.Draw(
                                texture,
                                transform.Position,
                                rect,
                                color,
                                transform.Rotation,
                                anchor,
                                transform.Scale * scaleMultiplier,
                                SpriteEffects.None,
                                renderer.Layer,
                                renderer.BlendState);
                        }
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
            else if (archetype.Has<AnimationRenderer>())
            {
                foreach (ref var chunk in archetype.GetChunksSpan())
                {
                    chunk.GetFilledComponentSpan<Transform, AnimationRenderer>(
                        out var transforms, out var renderers);

                    for (int i = 0; i < chunk.EntityCount; i++)
                    {
                        var transform = transforms.UnsafeAt(i);
                        var renderer = renderers.UnsafeAt(i);

                        batch.Draw(
                            renderer.Animation.Texture,
                            transform.Position,
                            renderer.CurrentFrame.SourceRect,
                            renderer.Color,
                            transform.Rotation,
                            renderer.CurrentFrame.Anchor,
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
