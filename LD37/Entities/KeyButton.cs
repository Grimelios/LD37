using System;
using LD37.Core;
using LD37.Interfaces;
using LD37.Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities
{
	internal class KeyButton : Entity, IInteractive
	{
		private const int InteractionSize = 32;

		private Sprite sprite;
		private MessageSystem messageSystem;

		public KeyButton(ContentLoader contentLoader, InteractionSystem interactionSystem, MessageSystem messageSystem)
		{
			this.messageSystem = messageSystem;

			sprite = new Sprite(contentLoader, "KeyButton", OriginLocations.Center);
			BoundingBox = new Rectangle(0, 0, InteractionSize, InteractionSize);
			interactionSystem.Items.Add(this);
		}

		public override Vector2 Position
		{
			set
			{
				sprite.Position = value;

				Rectangle boundingBox = BoundingBox;
				boundingBox.X = (int)value.X - boundingBox.Width / 2;
				boundingBox.Y = (int)value.Y - boundingBox.Height / 2;
				BoundingBox = boundingBox;

				base.Position = value;
			}
		}

		public Rectangle BoundingBox { get; private set; }
		
		public override bool TileAttach => true;

		public void InteractionResponse()
		{
			messageSystem.Send(new GameMessage(MessageTypes.LevelRefresh));
		}

		public override void Render(SpriteBatch sb)
		{
			sprite.Render(sb);
		}
	}
}
