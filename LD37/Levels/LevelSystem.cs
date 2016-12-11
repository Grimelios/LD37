using System.Collections.Generic;
using System.Linq;
using LD37.Entities;
using LD37.Entities.Abstract;
using LD37.Entities.Organization;
using LD37.Entities.Platforms;
using LD37.Input;
using LD37.Interfaces;
using LD37.Json;
using LD37.Messaging;
using LD37.Messaging.Input;
using LD37.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LD37.Levels
{
	internal class LevelSystem : IMessageReceiver
	{
		private const float DelayMultiplier = 1.5f;

		private int levelCounter;

		private InteractionSystem interactionSystem;
		private Scene scene;
		private Level currentLevel;
		private Tile[,] tiles;

		public LevelSystem(InteractionSystem interactionSystem, MessageSystem messageSystem, Scene scene)
		{
			this.interactionSystem = interactionSystem;
			this.scene = scene;

			RetrieveTiles();

			levelCounter = 7;

			messageSystem.Subscribe(MessageTypes.Keyboard, this);
			messageSystem.Subscribe(MessageTypes.LevelRefresh, this);
		}

		private void RetrieveTiles()
		{
			tiles = new Tile[Constants.RoomWidth - 2, Constants.RoomHeight - 2];

			List<Entity> tileList = scene.LayerMap["Primary"].EntityMap["Tile"];

			for (int i = 0; i < Constants.RoomHeight - 2; i++)
			{
				for (int j = 0; j < Constants.RoomWidth - 2; j++)
				{
					tiles[j, i] = (Tile)tileList[i * (Constants.RoomWidth - 2) + j];
				}
			}
		}

		public void Receive(GameMessage message)
		{
			switch (message.Type)
			{
				case MessageTypes.Keyboard:
					HandleKeyboard(((KeyboardMessage)message).Data);
					break;

				case MessageTypes.LevelRefresh:
					Refresh(((LevelRefreshMessage)message).TileCoordinates);
					break;
			}
		}

		private void HandleKeyboard(KeyboardData data)
		{
			if (data.KeysPressedThisFrame.Contains(Keys.R))
			{
				Refresh(Point.Zero, true, false);
			}
		}

		public void Refresh(Point sourceCoordinates, bool cascadeTiles = true, bool incrementLevel = true)
		{
			if (incrementLevel)
			{
				levelCounter++;
			}

			interactionSystem.Items.Clear();

			currentLevel?.Dispose();
			currentLevel = JsonUtilities.Deserialize<Level>("Levels/Level" + levelCounter + ".json");
			currentLevel.TileEntities.ForEach(entity => AttachToTile(entity, cascadeTiles));

			if (currentLevel.Platforms != null)
			{
				AttachPlatforms(currentLevel.Platforms, cascadeTiles);
			}

			WireElements(currentLevel.TileEntities);

			if (cascadeTiles)
			{
				CascadeTiles(sourceCoordinates);
			}
		}

		private void AttachPlatforms(List<Platform> platforms, bool cascadeTiles)
		{
			foreach (Platform platform in platforms)
			{
				PlatformSegment[,] segments = platform.Segments;

				for (int i = 0; i < platform.Height; i++)
				{
					for (int j = 0; j < platform.Width; j++)
					{
						AttachToTile(segments[j, i], cascadeTiles);
					}
				}
			}
		}

		private void AttachToTile(Entity entity, bool cascadeTiles)
		{
			Point tileCoordinates = TileConvert.ToTile(entity.Position);
			Tile tile = tiles[tileCoordinates.X, tileCoordinates.Y];
			tile.ReversedEntity = entity;

			if (!cascadeTiles)
			{
				tile.Flip();
			}
		}

		private void WireElements(List<Entity> entities)
		{
			foreach (Entity entity in entities)
			{
				AbstractPowerSource powerSource = entity as AbstractPowerSource;

				if (powerSource != null)
				{
					powerSource.PowerTargets = powerSource.TargetIDs.Select(id => entities[id]).Cast<IPowered>().ToArray();
				}
			}
		}

		private void CascadeTiles(Point sourceCoordinates)
		{
			Vector2 source = TileConvert.ToPixels(new Vector2(sourceCoordinates.X, sourceCoordinates.Y));

			for (int i = 0; i < Constants.RoomHeight - 2; i++)
			{
				for (int j = 0; j < Constants.RoomWidth - 2; j++)
				{
					Tile tile = tiles[j, i];
					tile.Flip(Vector2.Distance(source, tile.Position) * DelayMultiplier);
				}
			}
		}
	}
}
