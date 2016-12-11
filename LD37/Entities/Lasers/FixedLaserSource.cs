using System.Collections.Generic;
using LD37.Core;
using LD37.Entities.Organization;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities.Lasers
{
	using PropertyMap = Dictionary<string, string>;

	internal class FixedLaserSource : AbstractLaserSource
	{
		private static int sourceOffset;

		static FixedLaserSource()
		{
			PropertyMap properties = Properties.Load("Laser.properties");
			sourceOffset = int.Parse(properties["Fixed.Source.Offset"]);
		}

		private Sprite sprite;

		public FixedLaserSource(ContentLoader contentLoader, PhysicsHelper physicsHelper, PrimitiveDrawer primitiveDrawer, Scene scene) :
			base(physicsHelper, primitiveDrawer, scene)
		{
			sprite = new Sprite(contentLoader, "Lasers/FixedLaserSource", OriginLocations.Center);
			SourceOffset = sourceOffset;
		}

		public override Vector2 Position
		{
			set
			{
				sprite.Position = value;

				base.Position = value;
			}
		}

		public override Vector2 Scale
		{
			set
			{
				sprite.Scale = value;

				base.Scale = value;
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
