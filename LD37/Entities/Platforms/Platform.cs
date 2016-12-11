using FarseerPhysics.Dynamics;
using LD37.Entities.Abstract;
using LD37.Physics;
using LD37.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace LD37.Entities.Platforms
{
	internal class Platform : Entity
	{
		private Body body;
		private ContentLoader contentLoader;
		private PhysicsFactory physicsFactory;

		private int height;

		public Platform(ContentLoader contentLoader, PhysicsFactory physicsFactory)
		{
			this.contentLoader = contentLoader;
			this.physicsFactory = physicsFactory;
		}

		public PlatformSegment[,] Segments { get; private set; }

		[JsonProperty(Order = 2)]
		public override Vector2 Position
		{
			set
			{
				body.Position = value + new Vector2(Width, height) / 2 + Vector2.One;
				Segments = new PlatformSegment[Width, height];

				for (int i = 0; i < height; i++)
				{
					for (int j = 0; j < Width; j++)
					{
						Segments[j, i] = new PlatformSegment(contentLoader)
						{
							Position = TileConvert.ToPixels(value + new Vector2(j, i))
						};
					}
				}

				contentLoader = null;
			}
		}

		[JsonProperty]
		public int Width { get; set; }

		[JsonProperty(Order = 1)]
		public int Height
		{
			get { return height; }
			set
			{
				Vector2 center = PhysicsConvert.ToMeters(Position) + new Vector2(Width, value) / 2;

				height = value;
				body = physicsFactory.CreateRectangle(Width, value, center, Units.Meters, BodyType.Kinematic, this);
				physicsFactory = null;
			}
		}

		public override void Dispose()
		{
			body.Dispose();
		}

		public override void Render(SpriteBatch sb)
		{
		}
	}
}
