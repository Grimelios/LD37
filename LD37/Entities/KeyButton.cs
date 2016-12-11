using LD37.Core;
using LD37.Entities.Abstract;
using LD37.Interfaces;
using LD37.Messaging;
using LD37.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace LD37.Entities
{
	internal class KeyButton : Entity, IInteractive, IPowered
	{
		private const int InteractionSize = 32;

		private Sprite sprite;
		private MessageSystem messageSystem;

		public KeyButton(ContentLoader contentLoader, InteractionSystem interactionSystem, MessageSystem messageSystem)
		{
			this.messageSystem = messageSystem;

			sprite = new Sprite(contentLoader, "KeyButton", OriginLocations.Center);
			InteractionBox = new Rectangle(0, 0, InteractionSize, InteractionSize);
			interactionSystem.Items.Add(this);
			Powered = true;
		}

		public override Vector2 Position
		{
			set
			{
				sprite.Position = value;

				Rectangle box = InteractionBox;
				box.X = (int)value.X - box.Width / 2;
				box.Y = (int)value.Y - box.Height / 2;
				InteractionBox = box;

				base.Position = value;
			}
		}

		public override Vector2 Scale
		{
			set { sprite.Scale = value; }
		}

		[JsonIgnore]
		public Rectangle InteractionBox { get; private set; }
		
		[JsonProperty]
		public bool Powered { get; set; }

		public void InteractionResponse()
		{
			if (Powered)
			{
				messageSystem.Send(new LevelRefreshMessage(TileConvert.ToTile(Position)));
			}
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
