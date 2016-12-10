using System.Collections.Generic;
using LD37.Entities;
using Newtonsoft.Json;

namespace LD37.Levels
{
	internal class Level
	{
		[JsonConstructor]
		public Level(string name, List<Entity> entities)
		{
			Name = name;
			Entities = entities;
		}

		public string Name { get; }

		public List<Entity> Entities { get; }
	}
}
