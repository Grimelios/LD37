using System.Collections.Generic;
using LD37.Entities;
using LD37.Entities.Organization;
using LD37.Interfaces;
using LD37.Json;
using LD37.Messaging;
using LD37.Utility;
using Microsoft.Xna.Framework;

namespace LD37.Levels
{
	using EntityMap = Dictionary<string, List<Entity>>;

	internal class LevelSystem : IMessageReceiver
	{
		private const int DelayMultiplier = 2;

		private int levelCounter;

		private InteractionSystem interactionSystem;
		private Scene scene;
		private Tile[,] tiles;

		public LevelSystem(InteractionSystem interactionSystem, MessageSystem messageSystem, Scene scene)
		{
			this.interactionSystem = interactionSystem;
			this.scene = scene;

			RetrieveTiles();

			levelCounter = 1;
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
			Refresh(((LevelRefreshMessage)message).TileCoordinates);
		}

		public void Refresh(Point sourceCoordinates, bool cascadeTiles = true)
		{
			Level level = JsonUtilities.Deserialize<Level>("Levels/Level" + levelCounter + ".json");
			levelCounter++;

			EntityMap entityMap = scene.LayerMap["Primary"].EntityMap;

			foreach (Entity entity in level.Entities)
			{
				if (entity.TileAttach)
				{
					Point tileCoordinates = TileConvert.ToTile(entity.Position);
					Tile tile = tiles[tileCoordinates.X, tileCoordinates.Y];
					tile.ReversedEntity = entity;

					if (!cascadeTiles)
					{
						tile.Flip();
					}
				}
				else
				{
					entityMap[entity.EntityGroup].Add(entity);
				}
			}

			if (cascadeTiles)
			{
				CascadeTiles(sourceCoordinates);
				interactionSystem.Items.Clear();
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
