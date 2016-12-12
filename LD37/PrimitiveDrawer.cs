using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD37
{
	public class PrimitiveDrawer
	{
		private Texture2D pixelTexture;

		public PrimitiveDrawer(ContentLoader contentLoader)
		{
			pixelTexture = contentLoader.LoadTexture("Pixel");
		}
		
		public void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end, Color color)
		{
			float length = Vector2.Distance(start, end);
			float rotation = GameFunctions.ComputeAngle(start, end);

			Rectangle sourceRect = new Rectangle(0, 0, (int)length, 1);

			sb.Draw(pixelTexture, start, sourceRect, color, rotation, Vector2.Zero, 1, SpriteEffects.None, 0);
		}

		public void DrawRectangle(SpriteBatch sb, Rectangle rect, Color color)
		{
			Vector2 topLeft = new Vector2(rect.Left, rect.Top);
			Vector2 topRight = new Vector2(rect.Right, rect.Top);
			Vector2 bottomLeft = new Vector2(rect.Left, rect.Bottom);
			Vector2 bottomRight = new Vector2(rect.Right, rect.Bottom);

			DrawLine(sb, topLeft, topRight, color);
			DrawLine(sb, topLeft, bottomLeft, color);
			DrawLine(sb, topRight, bottomRight, color);
			DrawLine(sb, bottomLeft, bottomRight, color);
		}
	}
}
