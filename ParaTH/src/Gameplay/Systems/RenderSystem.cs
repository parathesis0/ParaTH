using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed class RenderSystem(World world, StgBatch batch)
{
    private QueryDescriptor query = new QueryDescriptor()
        .WithAll<Transform>()
        .WithAny<SpriteRenderer>(); // all renderers

    public void Update()
    {
        var q = world.GetOrCreateQuery(query);

        foreach (var archetype in q.GetMatchingArchetypesSpan())
        {
            if (archetype.Has<SpriteRenderer>())
            {
                foreach (ref var chunk in archetype.GetChunksSpan())
                {
                    chunk.GetFilledComponentSpan<Transform, SpriteRenderer>(out var transforms, out var renderers);

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
