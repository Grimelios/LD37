using LD37.Physics;
using Microsoft.Xna.Framework;

namespace LD37.Entities.Lasers
{
	internal abstract class AbstractLaserSource : Entity
	{
		protected AbstractLaserSource(PhysicsHelper physicsHelper, PrimitiveDrawer primitiveDrawer)
		{
			Laser = new Laser(physicsHelper, primitiveDrawer);
		}

		public Laser Laser { get; }

		public override Vector2 Position
		{
			set
			{
				Laser.RecomputePoints(value, Rotation);

				base.Position = value;
			}
		}

		public virtual Color Color
		{
			set { Laser.Color = value; }
		}

		public override float Rotation
		{
			set
			{
				Laser.RecomputePoints(Position, value);

				base.Rotation = value;
			}
		}
	}
}
