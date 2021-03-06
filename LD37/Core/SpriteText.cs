﻿using LD37.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Core
{
	internal  class SpriteText : IRenderable
	{
		private SpriteFont font;
		private Vector2 origin;

		public SpriteText(SpriteFont font, string value, Vector2 position, Color color)
		{
			this.font = font;

			Position = position;
			Value = value;
			Scale = 1;
			Color = color;
		}

		public Vector2 Position { get; set; }
		public Color Color { get; set; }

		public float Rotation { get; set; }
		public float Scale { get; set; }

		public string Value { get; set; }

		public void Render(SpriteBatch sb)
		{
			sb.DrawString(font, Value, Position, Color, Rotation, origin, Scale, SpriteEffects.None, 0);
		}
	}
}
