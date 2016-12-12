using LD37.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Core
{
	internal enum OriginLocations
	{
		Center,
		Default,
		Custom
	}

	internal class Sprite : IRenderable
	{
		private Texture2D texture;

		public Sprite(ContentLoader contentLoader, string filename, OriginLocations originLocation) :
			this(contentLoader.LoadTexture(filename), Vector2.Zero, null, originLocation)
		{
		}

		public Sprite(Texture2D texture, Vector2 origin, Rectangle? sourceRect) :
			this(texture, origin, sourceRect, OriginLocations.Custom)
		{
		}

		public Sprite(Texture2D texture, Rectangle? sourceRect, OriginLocations originLocation) :
			this(texture, Vector2.Zero, sourceRect, originLocation)
		{
		}

		public Sprite(Texture2D texture, Vector2 origin, Rectangle? sourceRect, OriginLocations originLocation)
		{
			this.texture = texture;

			if (originLocation == OriginLocations.Center)
			{
				Origin = sourceRect == null ? new Vector2(texture.Width, texture.Height) / 2
					: new Vector2(sourceRect.Value.Width, sourceRect.Value.Height) / 2;
			}
			else
			{
				Origin = origin;
			}

			SourceRect = sourceRect;
			Scale = Vector2.One;
			Color = Color.White;
		}

		public Rectangle? SourceRect { get; set; }
		public Vector2 Position { get; set; }
		public Vector2 Origin { get; set; }
		public Vector2 Scale { get; set; }
		public Color Color { get; set; }

		public float Rotation { get; set; }

		public SpriteEffects Effects { get; set; }

		public void Render(SpriteBatch sb)
		{
			sb.Draw(texture, Position, SourceRect, Color, Rotation, Origin, Scale, Effects, 0);
		}
	}
}
