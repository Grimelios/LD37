using Microsoft.Xna.Framework;

namespace LD37.Utility
{
	internal static class TileConvert
	{
		public static Vector2 ToPixels(Point point)
		{
			return ToPixels(new Vector2(point.X, point.Y));
		}

		public static Vector2 ToPixels(Vector2 position)
		{
			return position * Constants.TileSize + new Vector2(Constants.TileSize) * 1.5f;
		}

		public static Point ToTile(Vector2 position)
		{
			Vector2 tileCoordinates = (position - new Vector2(Constants.TileSize) * 1.5f) / Constants.TileSize;

			return new Point((int)tileCoordinates.X, (int)tileCoordinates.Y);
		}
	}
}
