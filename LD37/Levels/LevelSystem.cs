using LD37.Interfaces;
using LD37.Json;
using LD37.Messaging;

namespace LD37.Levels
{
	internal class LevelSystem : IMessageReceiver
	{
		private int levelCounter;

		public LevelSystem(MessageSystem messageSystem)
		{
			messageSystem.Subscribe(MessageTypes.LevelRefresh, this);
		}

		public void Receive(GameMessage message)
		{
			Level level = JsonUtilities.Deserialize<Level>("Level" + levelCounter + ".json");
			levelCounter++;
		}
	}
}
