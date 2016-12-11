using LD37.Entities;
using LD37.Entities.Abstract;
using LD37.Entities.Lasers;
using LD37.Entities.Organization;
using LD37.Input;
using LD37.Interfaces;
using LD37.Messaging;
using LD37.Messaging.Input;
using LD37.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Ninject;

namespace LD37
{
	internal class Editor : IMessageReceiver
	{
		private enum EditableEntityTypes
		{
			FixedLaserSource,
			LaserReceiver,
			KeyButton,
			Mirror,
			Switch
		}

		private Tile[,] tiles;
		private MessageSystem messageSystem;
		private StandardKernel kernel;

		private EditableEntityTypes selectedEntityType;

		public Editor(MessageSystem messageSystem, Scene scene, StandardKernel kernel)
		{
			this.messageSystem = messageSystem;
			this.kernel = kernel;

			tiles = scene.RetrieveTiles();

			messageSystem.Subscribe(MessageTypes.Keyboard, this);
			messageSystem.Subscribe(MessageTypes.Mouse, this);
		}

		public void Receive(GameMessage message)
		{
			switch (message.Type)
			{
				case MessageTypes.Keyboard:
					HandleKeyboard(((KeyboardMessage)message).Data);
					break;

				case MessageTypes.Mouse:
					HandleMouse(((MouseMessage)message).Data);
					break;
			}
		}

		private void HandleKeyboard(KeyboardData data)
		{
			foreach (Keys key in data.KeysPressedThisFrame)
			{
				selectedEntityType = GetEntityType(key);
			}

			bool controlHeld = false;

			foreach (Keys key in data.KeysDown)
			{
				if (key == Keys.LeftControl || key == Keys.RightControl)
				{
					controlHeld = true;

					break;
				}
			}

			if (controlHeld && data.KeysPressedThisFrame.Contains(Keys.S))
			{
				messageSystem.Send(new GameMessage(MessageTypes.LevelSave));
			}
		}

		private EditableEntityTypes GetEntityType(Keys key)
		{
			switch (key)
			{
				case Keys.F: return EditableEntityTypes.FixedLaserSource;
				case Keys.R: return EditableEntityTypes.LaserReceiver;
				case Keys.K: return EditableEntityTypes.KeyButton;
				case Keys.M: return EditableEntityTypes.Mirror;
				case Keys.S: return EditableEntityTypes.Switch;
			}

			return selectedEntityType;
		}

		private void HandleMouse(MouseData data)
		{
			Vector2 mousePosition = data.WorldPosition;

			int screenRight = Constants.RoomWidth * Constants.TileSize;
			int screenHeight = Constants.RoomHeight * Constants.TileSize;

			if (mousePosition.X < Constants.TileSize || mousePosition.X > screenRight - Constants.TileSize ||
			    mousePosition.Y < Constants.TileSize || mousePosition.Y > screenHeight - Constants.TileSize)
			{
				return;
			}

			Point tileCoordinates = new Point((int)mousePosition.X / Constants.TileSize - 1,
				(int)mousePosition.Y / Constants.TileSize - 1);
			Tile tile = tiles[tileCoordinates.X, tileCoordinates.Y];

			if (data.LeftClickState == ClickStates.PressedThisFrame)
			{
				Entity entity = CreateEntity();
				entity.Position = TileConvert.ToPixels(tileCoordinates);
				tile.AttachedEntity = entity;
			}
			else if (data.RightClickState == ClickStates.PressedThisFrame && tile.AttachedEntity != null)
			{
				tile.AttachedEntity.Dispose();
				tile.AttachedEntity = null;
			}
		}

		private Entity CreateEntity()
		{
			switch (selectedEntityType)
			{
				case EditableEntityTypes.FixedLaserSource: return kernel.Get<FixedLaserSource>();
				case EditableEntityTypes.LaserReceiver: return kernel.Get<LaserReceiver>();
				case EditableEntityTypes.KeyButton: return kernel.Get<KeyButton>();
				case EditableEntityTypes.Mirror: return kernel.Get<Mirror>();
				case EditableEntityTypes.Switch: return kernel.Get<Switch>();
			}

			return null;
		}
	}
}
