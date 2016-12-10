using Microsoft.Xna.Framework;

namespace LD37.Messaging
{
	internal class LevelRefreshMessage : GameMessage
	{
		public LevelRefreshMessage(Point tileCoordinates) : base(MessageTypes.LevelRefresh)
		{
			TileCoordinates = tileCoordinates;
		}

		public Point TileCoordinates { get; }
	}
}
