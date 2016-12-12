using System.Collections.Generic;
using LD37.Entities.Platforms;

namespace LD37.Messaging
{
	internal class LevelSaveMessage : GameMessage
	{
		public LevelSaveMessage(List<Platform> platforms) : base(MessageTypes.LevelSave)
		{
			Platforms = platforms;
		}

		public List<Platform> Platforms { get; }
	}
}
