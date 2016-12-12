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
	internal class SlidingDoor : Entity, IPowered
	{
		private const int SlideTime = 200;

		private Sprite frameSprite;
		private Sprite doorSprite;
		private Timer timer;
		private Body body;

		private Vector2 poweredDoorPosition;
		private Vector2 unpoweredDoorPosition;

		private bool powered;
		private bool positionSet;

		public SlidingDoor(ContentLoader contentLoader, PhysicsFactory physicsFactory)
		{
			Texture2D texture = contentLoader.LoadTexture("SlidingDoor");
			Rectangle frameRect = new Rectangle(0, 0, 32, texture.Height);
			Rectangle doorRect = new Rectangle(32, 0, 10, 64);
			Vector2 frameOrigin = new Vector2(16, 64);

			frameSprite = new Sprite(texture, frameOrigin, frameRect);
			doorSprite = new Sprite(texture, doorRect, OriginLocations.Center);
			body = physicsFactory.CreateRectangle(10, 64, Units.Pixels, BodyType.Static, this);
		}

		public override Vector2 Position
		{
			set
			{
				Vector2 halfTileVector = new Vector2(0, 16);

				frameSprite.Position = value - halfTileVector * 3;
				poweredDoorPosition = value - halfTileVector * 5;
				unpoweredDoorPosition = value - halfTileVector;
				doorSprite.Position = powered ? poweredDoorPosition : unpoweredDoorPosition;
				positionSet = true;

				base.Position = value;
			}
		}

		public override Vector2 Scale
		{
			set
			{
				frameSprite.Scale = value;
				doorSprite.Scale = value;

				base.Scale = value;
			}
		}

		[JsonProperty]
		public int PowerID { get; set; }

		[JsonProperty]
		public bool Powered
		{
			get { return powered; }
			set
			{
				powered = value;

				if (positionSet)
				{
					Vector2 start = powered ? unpoweredDoorPosition : poweredDoorPosition;
					Vector2 end = powered ? poweredDoorPosition : unpoweredDoorPosition;

					float initialElapsed = SlideTime - timer?.Elapsed ?? 0;

					timer = new Timer(SlideTime, (progress) =>
					{
						doorSprite.Position = Vector2.Lerp(start, end, progress);
					}, () =>
					{
						doorSprite.Position = end;
						timer = null;
					}, initialElapsed);
				}
			}
		}

		public Vector2 WirePosition => Position - new Vector2(0, Constants.HalfTile * 7);

		public override void Dispose()
		{
			body.Dispose();
		}

		public override void Update(float dt)
		{
			timer?.Update(dt);
			body.Position = PhysicsConvert.ToMeters(doorSprite.Position);
		}

		public override void Render(SpriteBatch sb)
		{
			frameSprite.Render(sb);
			doorSprite.Render(sb);
		}
	}
}
