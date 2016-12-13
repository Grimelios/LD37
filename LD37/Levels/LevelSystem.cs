using System.Collections.Generic;
using System.IO;
using System.Linq;
using LD37.Entities;
using LD37.Entities.Abstract;
using LD37.Entities.Organization;
using LD37.Entities.Platforms;
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
		private const float DelayMultiplier = 1.5f;
		private const string LevelDirectory = @"C:\Users\Mark\Documents\visual studio 2015\Projects\LD37\LD37\Content\Json\Levels\";

		private int levelCounter;
		private string levelFilename;

		private InteractionSystem interactionSystem;
		private MessageSystem messageSystem;
		private Level currentLevel;
		private Tile[,] tiles;
		private List<Entity> wires;

		public LevelSystem(InteractionSystem interactionSystem, MessageSystem messageSystem, Scene scene)
		{
			this.interactionSystem = interactionSystem;
			this.messageSystem = messageSystem;

			EntityMap entityMap = scene.LayerMap["Primary"].EntityMap;

			tiles = scene.RetrieveTiles();
			wires = entityMap["Wire"];
			levelCounter = 20;
			
			messageSystem.Subscribe(MessageTypes.LevelSave, this);
			messageSystem.Subscribe(MessageTypes.LevelRefresh, this);
		}

		public Editor Editor { get; set; }

		public void Receive(GameMessage message)
		{
			switch (message.Type)
			{
				case MessageTypes.LevelSave:
					SaveLevel((LevelSaveMessage)message);
					break;

				case MessageTypes.LevelRefresh:
					Refresh(((LevelRefreshMessage)message).TileCoordinates);
					break;
			}
		}

		private void SaveLevel(LevelSaveMessage message)
		{
			List<Entity> tileEntities = GetTileEntities();
			Level level = new Level("", tileEntities, message.Platforms, wires);

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
			wires.Clear();

			bool endGame = levelFilename == "LevelEnd.json";

			if (!endGame)
			{
				levelFilename = "Level" + levelCounter + ".json";

				if (!File.Exists(Paths.Json + "Levels/" + levelFilename))
				{
					levelFilename = "LevelEnd.json";
				}

				currentLevel?.Dispose();
				currentLevel = JsonUtilities.Deserialize<Level>("Levels/" + levelFilename);

				List<Entity> tileEntities = currentLevel.TileEntities;

				tileEntities.ForEach(entity => AttachToTile(entity, cascadeTiles));
				Editor.Platforms = currentLevel.Platforms ?? new List<Platform>();

				if (currentLevel.Platforms != null)
				{
					AttachPlatforms(currentLevel.Platforms, cascadeTiles);
				}

				List<IPowered> powerList = tileEntities.OfType<IPowered>().ToList();

				WireElements(tileEntities, powerList);
				CreateWires(powerList);
			}
			else
			{
				messageSystem.Send(new GameMessage(MessageTypes.EndGame));
			}

			if (cascadeTiles)
			{
				CascadeTiles(sourceCoordinates, endGame);
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

		private void WireElements(List<Entity> entities, List<IPowered> powerList)
		{
			foreach (Entity entity in entities)
			{
				AbstractPowerSource powerSource = entity as AbstractPowerSource;

				if (powerSource != null)
				{
					List<IPowered> powerTargets = powerSource.PowerTargets;

					foreach (int id in powerSource.TargetIDs)
					{
						powerTargets.AddRange(powerList.Where(poweredItem => poweredItem.PowerID == id));
					}
				}
			}
		}

		private void CreateWires(List<IPowered> powerList)
		{
			currentLevel.Wires?.ForEach(wire => wires.Add(wire));

			List<AbstractPowerSource> powerSources = powerList.OfType<AbstractPowerSource>().ToList();

			foreach (Wire wire in wires)
			{
				int targetID = wire.TargetID;

				foreach (AbstractPowerSource powerSource in powerSources)
				{
					if (powerSource.PowerID == targetID)
					{
						wire.Powered = powerSource.Powered;
						powerSource.Wires.Add(wire);
					}
				}
			}
		}

		private void CascadeTiles(Point sourceCoordinates, bool endGame)
		{
			Vector2 source = TileConvert.ToPixels(new Vector2(sourceCoordinates.X, sourceCoordinates.Y));

			int multiplier = endGame ? 2 : 1;

			for (int i = 0; i < Constants.RoomHeight - 2; i++)
			{
				for (int j = 0; j < Constants.RoomWidth - 2; j++)
				{
					Tile tile = tiles[j, i];
					tile.Flip(Vector2.Distance(source, tile.Position) * DelayMultiplier * multiplier, endGame);
				}
			}
		}
	}
}
