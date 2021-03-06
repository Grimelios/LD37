﻿using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using LD37.Core;
using LD37.Entities.Abstract;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

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

		private int powerCount;

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

		public override Vector2 WirePosition => Position - GameFunctions.ComputeDirection(Rotation) * Constants.HalfTile;

		public override float Rotation
		{
			get { return base.Rotation; }
			set
			{
				sprite.Rotation = value;
				body.Rotation = value;
				body.Position = ComputeBodyPosition(Position, value);

				base.Rotation = value;
			}
		}

		[JsonIgnore]
		public int PowerCount
		{
			get { return powerCount; }
			set
			{
				int previousPowerCount = powerCount;

				powerCount = MathHelper.Max(value, 0);

				if (powerCount == 0 && previousPowerCount > 0)
				{
					Powered = false;
				}
				else if (previousPowerCount == 0 && powerCount == 1)
				{
					Powered = true;
				}
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
