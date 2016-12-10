using LD37.Physics;
using Microsoft.Xna.Framework;

namespace LD37.Entities.Lasers
{
	internal abstract class AbstractLaserSource : Entity
	{
		private float rotation;

		protected AbstractLaserSource(PhysicsHelper physicsHelper, PrimitiveDrawer primitiveDrawer)
		{
			Laser = new Laser(physicsHelper, primitiveDrawer);
		}

		public Laser Laser { get; }

		public override Vector2 Position
		{
			set
			{
				Laser.RecomputePoints(value, rotation);

				base.Position = value;
			}
		}

		public virtual Color Color
		{
			set { Laser.Color = value; }
		}

		public virtual float Rotation
		{
			set
			{
				rotation = value;
				Laser.RecomputePoints(Position, value);
			}
		}
	}
}
