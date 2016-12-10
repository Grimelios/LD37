using LD37.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Core
{
	internal class Sprite : IRenderable
	{
		private Texture2D texture;

		public Sprite(ContentLoader contentLoader, string filename, OriginLocations originLocation)
		{
			texture = contentLoader.LoadTexture(filename);
			Origin = originLocation == OriginLocations.Center ? new Vector2(texture.Width, texture.Height) / 2 :
				Vector2.Zero;
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
