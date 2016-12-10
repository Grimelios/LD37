using LD37.Core;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities.Lasers
{
	internal class FixedLaserSource : AbstractLaserSource
	{
		private Sprite sprite;

		public FixedLaserSource(ContentLoader contentLoader, PhysicsHelper physicsHelper, PrimitiveDrawer primitiveDrawer) :
			base(physicsHelper, primitiveDrawer)
		{
			sprite = new Sprite(contentLoader, "FixedLaserSource", OriginLocations.Center);
		}

		public override Vector2 Position
		{
			set
			{
				sprite.Position = value;

				base.Position = value;
			}
		}

		public override float Rotation
		{
			set
			{
				sprite.Rotation = value;

				base.Rotation = value;
			}
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
