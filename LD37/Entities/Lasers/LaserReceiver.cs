using FarseerPhysics.Dynamics;
using LD37.Core;
using LD37.Entities.Abstract;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace LD37.Entities.Lasers
{
	internal class LaserReceiver : AbstractPowerSource
	{
		private Sprite sprite;
		private Body body;

		public LaserReceiver(ContentLoader contentLoader, PhysicsFactory physicsFactory)
		{
			sprite = new Sprite(contentLoader, "Lasers/LaserReceiver", OriginLocations.Center);
			body = physicsFactory.CreateRectangle(1, 1, Units.Meters, BodyType.Static, this);
		}

		public override Vector2 Position
		{
			set
			{
				body.Position = PhysicsConvert.ToMeters(value);
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

		public override void Dispose()
		{
			body.Dispose();
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
