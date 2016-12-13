using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using LD37.Core;
using LD37.Entities.Abstract;
using LD37.Interfaces;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace LD37.Entities
{
	using PropertyMap = Dictionary<string, string>;

	internal class Mirror : Entity, IPowered
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
			PowerID = AbstractPowerSource.NextID;
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
			get { return base.Rotation; }
			set
			{
				value = GameFunctions.ClampAngle(value);
				sprite.Rotation = value;

				if (!body.IsDisposed)
				{
					body.Rotation = value;
				}

				base.Rotation = value;
			}
		}

		[JsonProperty]
		public int PowerID { get; set; }

		public bool Powered { get; set; }

		[JsonIgnore]
		public Vector2 WirePosition => Position;

		public float? ComputeReflectionAngle(float incomingAngle)
		{
			int incomingSign = Math.Sign(incomingAngle);
			int mirrorSign = Math.Sign(Rotation);

			incomingSign = incomingSign == 0 ? 1 : incomingSign;
			incomingAngle -= MathHelper.Pi * (incomingSign == 0 ? 1 : incomingSign);
			incomingSign *= -1;

			float angleDifference = Math.Abs(Rotation - incomingAngle);

			if (Math.Abs(incomingAngle) > MathHelper.PiOver2 && Math.Abs(Rotation) > MathHelper.PiOver2 &&
			    ((incomingSign == 1 && mirrorSign == -1) || (incomingSign == -1 && mirrorSign == 1)))
			{
				angleDifference = MathHelper.TwoPi - angleDifference;
			}

			if (angleDifference > reflectionThreshold)
			{
				return null;
			}

			return GameFunctions.ClampAngle(Rotation * 2 - incomingAngle);
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
