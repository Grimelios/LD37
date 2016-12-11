using LD37.Core;
using LD37.Entities.Abstract;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace LD37.Entities.Lasers
{
	internal class LaserReceiver : AbstractPowerSource
	{
		private Sprite mainSprite;
		private Sprite fillSprite;
		private Color color;

		public LaserReceiver(ContentLoader contentLoader)
		{
			Texture2D texture = contentLoader.LoadTexture("Lasers/LaserReceiver");
			Rectangle mainRect = new Rectangle(0, 0, 32, 32);
			Rectangle fillRect = new Rectangle(32, 0, 32, 32);

			mainSprite = new Sprite(texture, mainRect, OriginLocations.Center);
			fillSprite = new Sprite(texture, fillRect, OriginLocations.Center);
		}

		public override Vector2 Position
		{
			set
			{
				mainSprite.Position = value;
				fillSprite.Position = value;

				base.Position = value;
			}
		}

		public override Vector2 Scale
		{
			set
			{
				mainSprite.Scale = value;
				fillSprite.Scale = value;

				base.Scale = value;
			}
		}

		[JsonProperty]
		public string Tint
		{
			set
			{
				color = GameFunctions.ParseColor(value) * 0.5f;
				fillSprite.Color = Powered ? color : color * 0.5f;
			}
		}

		public override float Rotation
		{
			set
			{
				mainSprite.Rotation = value;
				fillSprite.Rotation = value;

				base.Rotation = value;
			}
		}

		public override bool Powered
		{
			set
			{
				fillSprite.Color = Powered ? color : color * 0.5f;

				base.Powered = value;
			}
		}

		public override void Render(SpriteBatch sb)
		{
			mainSprite.Render(sb);
			fillSprite.Render(sb);
		}
	}
}
