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
	using EntityMap = Dictionary<string, List<Entity>>;

	internal class LevelSystem : IMessageReceiver
	{
		private const float DelayMultiplier = 1.5f;
		private const string LevelDirectory = @"C:\Users\Mark\Documents\visual studio 2015\Projects\LD37\LD37\Content\Json\Levels\";

		private int levelCounter;
		private string levelFilename;

		private InteractionSystem interactionSystem;
		private EntityMap entityMap;
		private Level currentLevel;
		private Tile[,] tiles;

		public LevelSystem(InteractionSystem interactionSystem, MessageSystem messageSystem, Scene scene)
		{
			this.interactionSystem = interactionSystem;

			tiles = scene.RetrieveTiles();
			entityMap = scene.LayerMap["Primary"].EntityMap;
			levelCounter = 10;

			messageSystem.Subscribe(MessageTypes.Keyboard, this);
			messageSystem.Subscribe(MessageTypes.LevelSave, this);
			messageSystem.Subscribe(MessageTypes.LevelRefresh, this);
		}

		public void Receive(GameMessage message)
		{
			switch (message.Type)
			{
				case MessageTypes.Keyboard:
					HandleKeyboard(((KeyboardMessage)message).Data);
					break;

				case MessageTypes.LevelSave:
					SaveLevel();
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
				//Refresh(Point.Zero, true, false);
			}
		}

		private void SaveLevel()
		{
			List<Entity> tileEntities = GetTileEntities();
			Level level = new Level("", tileEntities, currentLevel.Platforms, entityMap["Wire"]);

			JsonUtilities.Serialize(level, LevelDirectory + levelFilename);
		}

		private List<Entity> GetTileEntities()
		{
			List<Entity> tileEntities = new List<Entity>();

			for (int i = 0; i < Constants.RoomHeight - 2; i++)
			{
				for (int j = 0; j < Constants.RoomWidth - 2; j++)
				{
					Tile tile = tiles[j, i];

					if (tile.AttachedEntity != null && !(tile.AttachedEntity is PlatformSegment))
					{
						tileEntities.Add(tile.AttachedEntity);
					}
				}
			}

			return tileEntities;
		}

		public void Refresh(Point sourceCoordinates, bool cascadeTiles = true, bool incrementLevel = true)
		{
			if (incrementLevel)
			{
				levelCounter++;
			}

			interactionSystem.Items.Clear();
			entityMap["Wire"].Clear();

			levelFilename = "Level" + levelCounter + ".json";
			currentLevel?.Dispose();
			currentLevel = JsonUtilities.Deserialize<Level>("Levels/" + levelFilename);
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

			currentLevel.OtherEntities?.ForEach(entity => entityMap[entity.EntityGroup].Add(entity));
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
			List<IPowered> powerList = entities.OfType<IPowered>().OrderBy(item => item.PowerID).ToList();

			foreach (Entity entity in entities)
			{
				AbstractPowerSource powerSource = entity as AbstractPowerSource;

				if (powerSource?.TargetIDs != null)
				{
					powerSource.PowerTargets = powerSource.TargetIDs.Select(id => powerList[id]).ToList();
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
