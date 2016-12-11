using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using LD37.Core;
using LD37.Entities.Abstract;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities.Lasers
{
	using PropertyMap = Dictionary<string, string>;

	internal class LaserReceiver : AbstractPowerSource
	{
		private static float bodyWidth;
		private static float bodyHeight;
		private static float bodyOffset;

		static LaserReceiver()
		{
			PropertyMap properties = Properties.Load("Laser.properties");

			bodyWidth = PhysicsConvert.ToMeters(int.Parse(properties["Receiver.Body.Width"]));
			bodyHeight = PhysicsConvert.ToMeters(int.Parse(properties["Receiver.Body.Height"]));
			bodyOffset = PhysicsConvert.ToMeters(int.Parse(properties["Receiver.Body.Offset"]));
		}

		private Sprite sprite;
		private Body body;

		public LaserReceiver(ContentLoader contentLoader, PhysicsFactory physicsFactory)
		{
			sprite = new Sprite(contentLoader, "Lasers/LaserReceiver", OriginLocations.Center);
			body = physicsFactory.CreateRectangle(bodyWidth, bodyHeight, Units.Meters, BodyType.Static, this);
			body.IsSensor = true;
		}

		public override Vector2 Position
		{
			set
			{
				sprite.Position = value;
				body.Position = ComputeBodyPosition(value, Rotation);

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
				body.Rotation = value;
				body.Position = ComputeBodyPosition(Position, value);

				base.Rotation = value;
			}
		}

		private Vector2 ComputeBodyPosition(Vector2 position, float rotation)
		{
			return PhysicsConvert.ToMeters(position) + Vector2.Transform(new Vector2(bodyOffset, 0),
					Matrix.CreateRotationZ(rotation));
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
