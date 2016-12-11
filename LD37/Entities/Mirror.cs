using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using LD37.Core;
using LD37.Entities.Abstract;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities
{
	using PropertyMap = Dictionary<string, string>;

	internal class Mirror : Entity
	{
		private static float bodyLength;
		private static float reflectionThreshold;

		static Mirror()
		{
			PropertyMap properties = Properties.Load("Mirror.properties");

			bodyLength = PhysicsConvert.ToMeters(int.Parse(properties["Body.Length"]));
			reflectionThreshold = float.Parse(properties["Reflection.Threshold"]);
		}

		private Sprite sprite;
		private Body body;

		public Mirror(ContentLoader contentLoader, PhysicsFactory physicsFactory)
		{
			Vector2 halfVector = new Vector2(0, bodyLength / 2);

			sprite = new Sprite(contentLoader, "Mirror", OriginLocations.Center);
			body = physicsFactory.CreateEdge(halfVector, -halfVector, Units.Meters, this);
		}

		public override Vector2 Position
		{
			set
			{
				sprite.Position = value;
				body.Position = PhysicsConvert.ToMeters(value);

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

				base.Rotation = value;
			}
		}

		public float? ComputeReflectionAngle(float incomingAngle)
		{
			if (Math.Abs(Rotation - (incomingAngle - MathHelper.Pi * Math.Sign(incomingAngle))) > reflectionThreshold)
			{
				return null;
			}

			return MathHelper.Pi + Rotation * 2 - incomingAngle;
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
