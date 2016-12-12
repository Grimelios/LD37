using LD37.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Core
{
	internal  class SpriteText : IRenderable
	{
		private SpriteFont font;
		private Vector2 origin;

		private string value;
		
		public SpriteText(SpriteFont font, string value, Vector2 position, Color color)
		{
			this.font = font;
			this.value = value;

			Position = position;
			Scale = 1;
			Color = color;
		}

		public Vector2 Position { get; set; }
		public Color Color { get; set; }

		public float Rotation { get; set; }
		public float Scale { get; set; }

		public void Render(SpriteBatch sb)
		{
			sb.DrawString(font, value, Position, Color, Rotation, origin, Scale, SpriteEffects.None, 0);
		}
	}
}
