using System;
using Microsoft.Xna.Framework;

namespace LD37
{
	internal static class GameFunctions
	{
		public static int EnumCount(Type enumType)
		{
			return Enum.GetValues(enumType).Length;
		}

		public static float ComputeAngle(Vector2 vector)
		{
			return ComputeAngle(Vector2.Zero, vector);
		}

		public static float ComputeAngle(Vector2 start, Vector2 end)
		{
			float dX = end.X - start.X;
			float dY = end.Y - start.Y;

			return (float)Math.Atan2(dY, dX);
		}

		public static Vector2 ComputeDirection(float angle)
		{
			float x = (float)Math.Cos(angle);
			float y = (float)Math.Sin(angle);

			return new Vector2(x, y);
		}
	}
}
