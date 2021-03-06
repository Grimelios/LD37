﻿using LD37.Core;
using LD37.Entities.Organization;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities.Lasers
{
	internal class RotatingLaserSource : AbstractLaserSource
	{
		private Sprite sprite;

		public RotatingLaserSource(ContentLoader contentLoader, PhysicsHelper physicsHelper, PrimitiveDrawer primitiveDrawer, Scene scene) :
			base(physicsHelper, primitiveDrawer, scene)
		{
			sprite = new Sprite(contentLoader, "Lasers/RotatingLaserSource", OriginLocations.Center);
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
			Rotation += 0.01f;
			sprite.Render(sb);
		}
	}
}
