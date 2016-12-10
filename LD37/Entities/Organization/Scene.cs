using System.Collections.Generic;
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
