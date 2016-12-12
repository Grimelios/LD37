using System;
using System.Collections.Generic;
using LD37.Entities.Abstract;
using LD37.Entities.Platforms;
using Newtonsoft.Json;

namespace LD37.Levels
{
	internal class Level : IDisposable
	{
		[JsonConstructor]
		public Level(string name, List<Entity> tileEntities, List<Platform> platforms, List<Entity> wires)
		{
			Name = name;
			TileEntities = tileEntities;
			Platforms = platforms;
			Wires = wires;
		}

		public string Name { get; }

		public List<Entity> TileEntities { get; }
		public List<Platform> Platforms { get; }
		public List<Entity> Wires { get; }

		public void Dispose()
		{
			TileEntities.ForEach(entity => entity.Dispose());
			Platforms?.ForEach(platform => platform.Dispose());
		}
	}
}
