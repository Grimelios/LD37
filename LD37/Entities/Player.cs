using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using LD37.Core;
using LD37.Entities.Platforms;
using LD37.Input;
using LD37.Interfaces;
using LD37.Messaging;
using LD37.Messaging.Input;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LD37.Entities
{
	using PropertyMap = Dictionary<string, string>;

	internal class Player : Entity, IMessageReceiver
	{
		private InteractionSystem interactionSystem;
		private Sprite sprite;
		private Body body;
		private Rectangle boundingBox;

		private float acceleration;
		private float deceleration;
		private float maxSpeed;
		private float jumpSpeedInitial;
		private float jumpSpeedLimited;

		private bool jumpEnabled;

		private int movementSign;

		public Player(ContentLoader contentLoader, InteractionSystem interactionSystem, MessageSystem messageSystem,
			PhysicsFactory physicsFactory)
		{
			this.interactionSystem = interactionSystem;

			PropertyMap properties = Properties.Load("Player.properties");

			acceleration = PhysicsConvert.ToMeters(int.Parse(properties["Acceleration"]));
			deceleration = PhysicsConvert.ToMeters(int.Parse(properties["Deceleration"]));
			maxSpeed = PhysicsConvert.ToMeters(int.Parse(properties["Max.Speed"]));
			jumpSpeedInitial = PhysicsConvert.ToMeters(int.Parse(properties["Jump.Speed.Initial"]));
			jumpSpeedLimited = PhysicsConvert.ToMeters(int.Parse(properties["Jump.Speed.Limited"]));

			int width = int.Parse(properties["Width"]);
			int height = int.Parse(properties["Height"]);

			boundingBox = new Rectangle(0, 0, width, height);
			sprite = new Sprite(contentLoader, "Player", OriginLocations.Center);
			body = physicsFactory.CreateRectangle(width, height, Units.Pixels, BodyType.Dynamic, this);
			body.FixedRotation = true;
			body.Friction = 0;
			body.OnCollision += HandleCollision;

			acceleration *= body.Mass;
			deceleration *= body.Mass;
			jumpSpeedInitial *= body.Mass;

			messageSystem.Subscribe(MessageTypes.Keyboard, this);
		}

		public override Vector2 Position
		{
			set
			{
				sprite.Position = value;
				boundingBox.X = (int)value.X - boundingBox.Width / 2;
				boundingBox.Y = (int)value.Y - boundingBox.Height / 2;

				base.Position = value;
			}
		}

		public override Vector2 LoadPosition
		{
			set
			{
				body.Position = PhysicsConvert.ToMeters(value);
				Position = value;
			}
		}

		public override string EntityGroup => "Player";

		private bool HandleCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			Entity entity = fixtureB.Body.UserData as Entity;

			if (contact.Manifold.LocalNormal == -Vector2.UnitY && (entity is Tilemap || entity is Platform))
			{
				jumpEnabled = true;
			}

			return true;
		}

		public void Receive(GameMessage message)
		{
			switch (message.Type)
			{
				case MessageTypes.Keyboard:
					HandleKeyboard(((KeyboardMessage)message).Data);
					break;
			}
		}

		private void HandleKeyboard(KeyboardData data)
		{
			if (data.KeysPressedThisFrame.Contains(Keys.E))
			{
				interactionSystem.CheckInteraction(boundingBox);
			}

			HandleRunning(data);
			HandleJumping(data);
		}

		private void HandleRunning(KeyboardData data)
		{
			bool aDown = data.KeysDown.Contains(Keys.A);
			bool dDown = data.KeysDown.Contains(Keys.D);

			Vector2 force = Vector2.Zero;
			movementSign = Math.Sign(body.LinearVelocity.X);

			if (aDown ^ dDown)
			{
				force.X = aDown ? -acceleration : acceleration;
			}
			else if (movementSign != 0)
			{
				force.X = deceleration * -movementSign;
			}

			body.ApplyForce(force);
		}

		private void HandleJumping(KeyboardData data)
		{
			if (jumpEnabled)
			{
				if (data.KeysPressedThisFrame.Contains(Keys.Space))
				{
					body.ApplyLinearImpulse(new Vector2(0, -jumpSpeedInitial));
					jumpEnabled = false;
				}
			}
			else
			{
				Vector2 velocity = body.LinearVelocity;

				if (velocity.Y < -jumpSpeedLimited && data.KeysReleasedThisFrame.Contains(Keys.Space))
				{
					velocity.Y = -jumpSpeedLimited;
					body.LinearVelocity = velocity;
				}
			}
		}

		public override void Update(float dt)
		{
			if (movementSign != 0)
			{
				Vector2 velocity = body.LinearVelocity;

				if (Math.Abs(body.LinearVelocity.X) > maxSpeed)
				{
					velocity.X = maxSpeed * Math.Sign(velocity.X);
				}
				else if (Math.Sign(body.LinearVelocity.X) != movementSign)
				{
					velocity.X = 0;
				}

				body.LinearVelocity = velocity;
			}

			Position = PhysicsConvert.ToPixels(body.Position);
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
