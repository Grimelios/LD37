using LD37.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities.Platforms
{
	internal class PlatformSegment : Entity
	{
		private Sprite sprite;

		public PlatformSegment(ContentLoader contentLoader)
		{
			sprite = new Sprite(contentLoader, "Tilesheets/Platform", OriginLocations.Center);
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

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
