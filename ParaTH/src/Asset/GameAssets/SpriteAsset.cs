using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParaTH;

public sealed record SpriteAsset(Texture2D Texture, Rectangle SourceRect, Vector2 Anchor) : Asset;
