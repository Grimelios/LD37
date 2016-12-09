using Microsoft.Xna.Framework;

namespace LD37.Physics
{
	public class PhysicsConvert
	{
		private const int PixelsPerMeter = 48;

		public static float ToPixels(float value)
		{
			return value * PixelsPerMeter;
		}

		public static float ToMeters(float value)
		{
			return value / PixelsPerMeter;
		}

		public static Vector2 ToPixels(Vector2 value)
		{
			return value * PixelsPerMeter;
		}

		public static Vector2 ToMeters(Vector2 value)
		{
			return value / PixelsPerMeter;
		}
	}
}
