using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

// todos:
// refactor once there's more renderers, use modifier pattern maybe
public sealed class RenderSystem(World world, StgBatch batch)
{
    private QueryDescriptor descriptor = new QueryDescriptor()
        .WithAll<Transform, RenderState>()
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
                        chunk.GetFilledComponentSpan<Transform, RenderState, SpriteRenderer, SpawnAnimation>(
                            out var transforms, out var states, out var renderers, out var spawnAnims);

                        for (int i = 0; i < chunk.EntityCount; i++)
                        {
                            var spawnAnim = spawnAnims.UnsafeAt(i);
                            var transform = transforms.UnsafeAt(i);
                            var state = states.UnsafeAt(i);
                            var renderer = renderers.UnsafeAt(i);

                            var sprite = renderer.Sprite;
                            var scaleX = state.Scale.X;
                            var scaleY = state.Scale.Y;
                            var color = state.Color;

                            if (spawnAnim.Counter < spawnAnim.Duration)
                            {
                                sprite = spawnAnim.Sprite;
                                var t = (float)(spawnAnim.Counter + 1) / spawnAnim.Duration;
                                var tX = Easing.Evaluate(spawnAnim.TypeX, t);
                                var tY = Easing.Evaluate(spawnAnim.TypeY, t);
                                var startScale = spawnAnim.StartScale;
                                var startA = (float)spawnAnim.StartAlpha * 255;
                                scaleX = MathHelper.Lerp(startScale.X, state.Scale.X, tX);
                                scaleY = MathHelper.Lerp(startScale.Y, state.Scale.Y, tY);
                                color.A = (byte)MathHelper.Lerp(startA, state.Color.A, t);
                            }

                            batch.Draw(
                                sprite.Texture,
                                transform.Position,
                                sprite.SourceRect,
                                color,
                                state.Rotation,
                                sprite.Anchor,
                                new Vector2(scaleX, scaleY),
                                SpriteEffects.None,
                                state.Layer,
                                state.BlendState);
                        }
                    }
                }
                else if (archetype.Has<AnimationRenderer>())
                {
                    foreach (ref var chunk in archetype.GetChunksSpan())
                    {
                        chunk.GetFilledComponentSpan<Transform, RenderState, AnimationRenderer, SpawnAnimation>(
                            out var transforms, out var states, out var renderers, out var spawnAnims);

                        for (int i = 0; i < chunk.EntityCount; i++)
                        {
                            var spawnAnim = spawnAnims.UnsafeAt(i);
                            var transform = transforms.UnsafeAt(i);
                            var state = states.UnsafeAt(i);
                            var renderer = renderers.UnsafeAt(i);

                            var texture = renderer.Animation.Texture;
                            var rect = renderer.CurrentFrame.SourceRect;
                            var anchor = renderer.CurrentFrame.Anchor;
                            var scaleX = state.Scale.X;
                            var scaleY = state.Scale.Y;
                            var color = state.Color;

                            if (spawnAnim.Counter < spawnAnim.Duration)
                            {
                                texture = spawnAnim.Sprite.Texture;
                                rect = spawnAnim.Sprite.SourceRect;
                                anchor = spawnAnim.Sprite.Anchor;
                                var t = (float)(spawnAnim.Counter + 1) / spawnAnim.Duration;
                                var tX = Easing.Evaluate(spawnAnim.TypeX, t);
                                var tY = Easing.Evaluate(spawnAnim.TypeY, t);
                                var startScale = spawnAnim.StartScale;
                                var startA = (float)spawnAnim.StartAlpha * 255;
                                scaleX = MathHelper.Lerp(startScale.X, state.Scale.X, tX);
                                scaleY = MathHelper.Lerp(startScale.Y, state.Scale.Y, tY);
                                color.A = (byte)MathHelper.Lerp(startA, state.Color.A, t);
                            }

                            batch.Draw(
                                texture,
                                transform.Position,
                                rect,
                                color,
                                state.Rotation,
                                anchor,
                                new Vector2(scaleX, scaleY),
                                SpriteEffects.None,
                                state.Layer,
                                state.BlendState);
                        }
                    }
                }
            }
            else if (archetype.Has<SpriteRenderer>())
            {
                foreach (ref var chunk in archetype.GetChunksSpan())
                {
                    chunk.GetFilledComponentSpan<Transform, RenderState, SpriteRenderer>(
                        out var transforms, out var states, out var renderers);

                    for (int i = 0; i < chunk.EntityCount; i++)
                    {
                        var transform = transforms.UnsafeAt(i);
                        var state = states.UnsafeAt(i);
                        var renderer = renderers.UnsafeAt(i);

                        batch.Draw(
                            renderer.Sprite.Texture,
                            transform.Position,
                            renderer.Sprite.SourceRect,
                            state.Color,
                            transform.Rotation,
                            renderer.Sprite.Anchor,
                            state.Scale,
                            SpriteEffects.None,
                            state.Layer,
                            state.BlendState);
                    }
                }
            }
            else if (archetype.Has<AnimationRenderer>())
            {
                foreach (ref var chunk in archetype.GetChunksSpan())
                {
                    chunk.GetFilledComponentSpan<Transform, RenderState, AnimationRenderer>(
                        out var transforms, out var states, out var renderers);

                    for (int i = 0; i < chunk.EntityCount; i++)
                    {
                        var transform = transforms.UnsafeAt(i);
                        var state = states.UnsafeAt(i);
                        var renderer = renderers.UnsafeAt(i);

                        batch.Draw(
                            renderer.Animation.Texture,
                            transform.Position,
                            renderer.CurrentFrame.SourceRect,
                            state.Color,
                            state.Rotation,
                            renderer.CurrentFrame.Anchor,
                            state.Scale,
                            SpriteEffects.None,
                            state.Layer,
                            state.BlendState);
                    }
                }
            }
        }
    }
}
