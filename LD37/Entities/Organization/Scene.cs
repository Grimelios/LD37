using System.Collections.Generic;
using LD37.Entities.Abstract;
using LD37.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities.Organization
{
	using LayerMap = Dictionary<string, EntityLayer>;

	internal class Scene : IDynamic, IRenderable
	{
		public Scene()
		{
			LayerMap = new LayerMap();
		}

		public LayerMap LayerMap { get; }

		public Tile[,] RetrieveTiles()
		{
			Tile[,] tiles = new Tile[Constants.RoomWidth - 2, Constants.RoomHeight - 2];

			List<Entity> tileList = LayerMap["Primary"].EntityMap["Tile"];

			for (int i = 0; i < Constants.RoomHeight - 2; i++)
			{
				for (int j = 0; j < Constants.RoomWidth - 2; j++)
				{
					tiles[j, i] = (Tile)tileList[i * (Constants.RoomWidth - 2) + j];
				}
			}

			return tiles;
		}

		public void Update(float dt)
		{
			foreach (EntityLayer layer in LayerMap.Values)
			{
				layer.Update(dt);
			}
		}

		public void Render(SpriteBatch sb)
		{
			foreach (EntityLayer layer in LayerMap.Values)
			{
				layer.Render(sb);
			}
		}
	}
}
