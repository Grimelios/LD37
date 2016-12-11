using System.IO;
using Newtonsoft.Json;

namespace LD37.Json
{
	public static class JsonUtilities
	{
		public static void Serialize(object value, string filename)
		{
			File.WriteAllText(filename, JsonConvert.SerializeObject(value));
		}

		public static T Deserialize<T>(string filename)
		{
			return JsonConvert.DeserializeObject<T>(File.ReadAllText(Paths.Json + filename));
		}
	}
}
