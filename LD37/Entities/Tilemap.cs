using LD37.Entities.Abstract;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

namespace LD37.Entities
{
	internal class Tilemap : Entity
	{
		private ContentLoader contentLoader;
		private Texture2D tilesheet;
		
		private int tilesPerRow;

		public Tilemap(ContentLoader contentLoader)
		{
			this.contentLoader = contentLoader;
		}

		[JsonProperty]
		public int TileSize { get; set; }

		[JsonProperty]
		public int Width { get; set; }

		[JsonProperty]
		public int Height { get; set; }

		[JsonProperty]
		public int[,] Tiles { get; set; }

		[JsonProperty(Order = 1)]
		public string TilesheetFilename
		{
			set
			{
				tilesheet = contentLoader.LoadTexture("Tilesheets/" + value);
				tilesPerRow = tilesheet.Width / TileSize;
				contentLoader = null;
			}
		}

		public override void Render(SpriteBatch sb)
		{
			Vector2 tilePosition = Position;
			Rectangle sourceRect = new Rectangle(0, 0, TileSize, TileSize);

			for (int i = 0; i < Height; i++)
			{
				tilePosition.X = Position.X;

				for (int j = 0; j < Width; j++)
				{
					int tileValue = Tiles[i, j];

					if (tileValue != -1)
					{
						sourceRect.X = tileValue % tilesPerRow * TileSize;
						sourceRect.Y = tileValue / tilesPerRow * TileSize;

						sb.Draw(tilesheet, tilePosition, sourceRect, Color.White);
					}

					tilePosition.X += TileSize;
				}

				tilePosition.Y += TileSize;
			}
		}
	}
}
