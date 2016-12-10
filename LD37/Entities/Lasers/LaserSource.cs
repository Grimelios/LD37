using LD37.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities.Lasers
{
	internal class LaserSource : Entity
	{
		private Sprite sprite;
		private Laser laser;

		public LaserSource(ContentLoader contentLoader)
		{
			sprite = new Sprite(contentLoader, "LaserSource", OriginLocations.Center);
		}

		public override Vector2 Position
		{
			set { sprite.Position = value; }
		}

		public Color Color
		{
			set { laser.Color = value; }
		}

		public float Rotation
		{
			set { sprite.Rotation = value; }
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
