using System.Collections.Generic;
using LD37.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities
{
	using PropertyMap = Dictionary<string, string>;

	internal class Tile : Entity
	{
		private static int flipTime;

		static Tile()
		{
			PropertyMap properties = Properties.Load("Miscellaneous.properties");
			flipTime = int.Parse(properties["Tile.Flip.Time"]);
		}

		private Sprite sprite;
		private Timer timer;

		public Tile(ContentLoader contentLoader)
		{
			sprite = new Sprite(contentLoader, "Tile", OriginLocations.Center);
		}

		public override Vector2 Position
		{
			set { sprite.Position = value; }
		}

		public override Vector2 Scale
		{
			set
			{
				sprite.Scale = value;

				if (AttachedEntity != null)
				{
					AttachedEntity.Scale = value;
				}

				if (ReversedEntity != null)
				{
					ReversedEntity.Scale = value;
				}
			}
		}

		public Entity AttachedEntity { get; set; }
		public Entity ReversedEntity { get; set; }

		public void Flip()
		{
			timer = new Timer(flipTime, (progress) =>
			{
				Scale = new Vector2(1 - progress * progress, 1);
			}, ReverseFlip, true);
		}

		private void ReverseFlip()
		{
			// TODO: Remove the attached entity from the world.
			AttachedEntity = ReversedEntity;
			ReversedEntity = null;

			timer = new Timer(flipTime, (progress) =>
			{
				Scale = new Vector2(-progress * (progress - 2), 1);
			}, () =>
			{
				Scale = Vector2.One;
				timer = null;
			}, false, timer.Elapsed);
		}

		public override void Update(float dt)
		{
			timer?.Update(dt);
			AttachedEntity?.Update(dt);
			ReversedEntity?.Update(dt);
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
			AttachedEntity?.Render(sb);
		}
	}
}
