using System.Collections.Generic;
using System.IO;

namespace LD37
{
	using PropertyMap = Dictionary<string, string>;

	public static class Properties
	{
		// TODO: Clear cache when/where appropriate.
		private static Dictionary<string, PropertyMap> propertyCache = new Dictionary<string, PropertyMap>();

		public static PropertyMap Load(string filename)
		{
			PropertyMap properties;
			
			if (propertyCache.TryGetValue(filename, out properties))
			{
				return properties;
			}

			properties = new PropertyMap();

			foreach (string line in File.ReadAllLines(Paths.Properties + filename))
			{
				if (line != "")
				{
					string[] tokens = line.Split('=');
					properties.Add(tokens[0], tokens[1]);
				}
			}

			propertyCache.Add(filename, properties);

			return properties;
		}
	}
}
