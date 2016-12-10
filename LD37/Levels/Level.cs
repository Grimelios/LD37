using System;
using System.Collections.Generic;
using LD37.Entities;
using LD37.Entities.Platforms;
using Newtonsoft.Json;

namespace LD37.Levels
{
	internal class Level : IDisposable
	{
		[JsonConstructor]
		public Level(string name, List<Entity> tileEntities, List<Platform> platforms)
		{
			Name = name;
			TileEntities = tileEntities;
			Platforms = platforms;
		}

		public string Name { get; }

		public List<Entity> TileEntities { get; }
		public List<Platform> Platforms { get; }

		public void Dispose()
		{
			TileEntities.ForEach(entity => entity.Dispose());
			Platforms?.ForEach(platform => platform.Dispose());
		}
	}
}
