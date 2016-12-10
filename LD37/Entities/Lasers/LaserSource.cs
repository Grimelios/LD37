using LD37.Core;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities.Lasers
{
	internal class LaserSource : Entity
	{
		private Sprite sprite;

		private float rotation;

		public LaserSource(ContentLoader contentLoader, PhysicsHelper physicsHelper, PrimitiveDrawer primitiveDrawer)
		{
			sprite = new Sprite(contentLoader, "LaserSource", OriginLocations.Center);
			Laser = new Laser(physicsHelper, primitiveDrawer);
		}

		public Laser Laser { get; }

		public override Vector2 Position
		{
			set
			{
				sprite.Position = value;
				Laser.RecomputePoints(value, rotation);

				base.Position = value;
			}
		}

		public Color Color
		{
			set { Laser.Color = value; }
		}

		public float Rotation
		{
			set
			{
				rotation = value;
				sprite.Rotation = value;
				Laser.RecomputePoints(Position, value);
			}
		}

		public override void Render(SpriteBatch sb)
		{
			Rotation = rotation + 0.01f;

			sprite.Render(sb);
		}
	}
}
