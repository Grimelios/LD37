﻿using System;
using System.Collections.Generic;
using LD37.Core;
using LD37.Entities.Abstract;
using LD37.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities
{
	using PropertyMap = Dictionary<string, string>;

	internal class Rotator : AbstractPowerSource, IInteractive
	{
		private static float angularAcceleration;
		private static float angularDeceleration;
		private static float angularMaxSpeed;

		static Rotator()
		{
			PropertyMap properties = Properties.Load("Rotator.properties");
			angularAcceleration = float.Parse(properties["Angular.Acceleration"]);
			angularDeceleration = float.Parse(properties["Angular.Deceleration"]);
			angularMaxSpeed = float.Parse(properties["Angular.Max.Speed"]);
		}

		private Sprite mainSprite;
		private Sprite innerSprite;

		private bool accelerating;
		private float angularVelocity;

		public Rotator(ContentLoader contentLoader, InteractionSystem interactionSystem)
		{
			Texture2D texture = contentLoader.LoadTexture("Rotator");
			Rectangle mainRect = new Rectangle(0, 0, 32, 32);
			Rectangle innerRect = new Rectangle(32, 0, 12, 12);

			mainSprite = new Sprite(texture, mainRect, OriginLocations.Center);
			innerSprite = new Sprite(texture, innerRect, OriginLocations.Center);
			InteractionBox = new Rectangle(0, 0, 32, 32);

			interactionSystem.Items.Add(this);
		}

		public Rectangle InteractionBox { get; private set; }

		public override Vector2 Position
		{
			set
			{
				mainSprite.Position = value;
				innerSprite.Position = value;

				Rectangle box = InteractionBox;
				box.X = (int)value.X - box.Width / 2;
				box.Y = (int)value.Y - box.Height / 2;
				InteractionBox = box;

				base.Position = value;
			}
		}

		public override Vector2 Scale
		{
			set
			{
				mainSprite.Scale = value;
				innerSprite.Scale = value;

				base.Scale = value;
			}
		}

		public override float Rotation
		{
			set
			{
				float previousRotation = Rotation;

				value = GameFunctions.ClampAngle(value);
				innerSprite.Rotation = value;

				if (Powered)
				{
					float rotationChange = value - previousRotation;

					foreach (IPowered target in PowerTargets)
					{
						((Entity)target).Rotation += rotationChange;
					}
				}

				base.Rotation = value;
			}
		}

		public override Vector2 WirePosition => Position;

		public void RotateLeft()
		{
			if (!Powered)
			{
				return;
			}

			angularVelocity -= angularAcceleration;
			angularVelocity = angularVelocity < -angularMaxSpeed ? -angularMaxSpeed : angularVelocity;
			accelerating = true;
		}

		public void RotateRight()
		{
			if (!Powered)
			{
				return;
			}

			angularVelocity += angularAcceleration;
			angularVelocity = angularVelocity > angularMaxSpeed ? angularMaxSpeed : angularVelocity;
			accelerating = true;
		}

		public void InteractionResponse()
		{
		}

		public override void Update(float dt)
		{
			if (!accelerating && angularVelocity != 0)
			{
				int previousSign = Math.Sign(angularVelocity);

				angularVelocity -= angularDeceleration * previousSign;

				if (Math.Sign(angularVelocity) != previousSign)
				{
					angularVelocity = 0;
				}
			}

			Rotation += angularVelocity * dt;
			accelerating = false;
		}

		public override void Render(SpriteBatch sb)
		{
			mainSprite.Render(sb);
			innerSprite.Render(sb);
		}
	}
}
