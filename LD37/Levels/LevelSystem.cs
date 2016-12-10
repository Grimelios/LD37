using System.Collections.Generic;
using LD37.Entities;
using LD37.Entities.Organization;
using LD37.Interfaces;
using LD37.Json;
using LD37.Messaging;
using Microsoft.Xna.Framework;

namespace LD37.Levels
{
	using EntityMap = Dictionary<string, List<Entity>>;

	internal class LevelSystem : IMessageReceiver
	{
		private int levelCounter;

		private Scene scene;
		private Tile[,] tiles;

		public LevelSystem(MessageSystem messageSystem)
		{
			levelCounter = 1;
			messageSystem.Subscribe(MessageTypes.LevelRefresh, this);
		}

		public Scene Scene
		{
			set
			{
				scene = value;
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
		}

		public void Receive(GameMessage message)
		{
			Refresh();
		}

		public void Refresh()
		{
			Level level = JsonUtilities.Deserialize<Level>("Levels/Level" + levelCounter + ".json");
			levelCounter++;

			EntityMap entityMap = scene.LayerMap["Primary"].EntityMap;

			foreach (Entity entity in level.Entities)
			{
				if (entity.TileAttach)
				{
					Vector2 tileCoordinates = (entity.Position - new Vector2(Constants.TileSize) * 1.5f) / Constants.TileSize;
					Tile tile = tiles[(int)tileCoordinates.X, (int)tileCoordinates.Y];
					tile.ReversedEntity = entity;
					tile.Flip();
				}
				else
				{
					entityMap[entity.EntityGroup].Add(entity);
				}
			}
		}
	}
}
