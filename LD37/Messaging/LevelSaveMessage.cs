using System.Collections.Generic;
using LD37.Entities.Platforms;

namespace LD37.Messaging
{
	internal class LevelSaveMessage : GameMessage
	{
		public LevelSaveMessage(List<Platform> createdPlatforms) : base(MessageTypes.LevelSave)
		{
			CreatedPlatforms = createdPlatforms;
		}

		public List<Platform> CreatedPlatforms { get; }
	}
}
