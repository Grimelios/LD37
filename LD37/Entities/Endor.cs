using LD37.Core;
using LD37.Entities.Abstract;
using LD37.Interfaces;
using LD37.Messaging;
using LD37.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities
{
	internal class Endor : Entity, IInteractive
	{
		private Sprite sprite;
		private MessageSystem messageSystem;

		public Endor(ContentLoader contentLoader, InteractionSystem interactionSystem, MessageSystem messageSystem)
		{
			this.messageSystem = messageSystem;

			Texture2D texture = contentLoader.LoadTexture("Endor");

			sprite = new Sprite(texture, new Vector2(Constants.HalfTile * 5, Constants.HalfTile * 9));
			InteractionBox = new Rectangle(0, 0, texture.Width, texture.Height);

			interactionSystem.Items.Add(this);
		}

		public override Vector2 Position
		{
			set
			{
				sprite.Position = value;

				Rectangle box = InteractionBox;
				box.X = (int)value.X - Constants.TileSize - box.Width / 2;
				box.Y = (int)value.Y - Constants.TileSize * 2 - box.Height / 2;
				InteractionBox = box;

				base.Position = value;
			}
		}

		public override Vector2 Scale
		{
			set { sprite.Scale = value; }
		}

		public Rectangle InteractionBox { get; private set; }

		public void InteractionResponse()
		{
			messageSystem.Send(new LevelRefreshMessage(TileConvert.ToTile(Position)));
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
