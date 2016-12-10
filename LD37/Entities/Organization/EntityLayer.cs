using System.Collections.Generic;
using LD37.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities.Organization
{
	using EntityMap = Dictionary<string, List<Entity>>;

	internal class EntityLayer : IDynamic, IRenderable
	{
		private string[] updateOrder;
		private string[] renderOrder;

		public EntityLayer(string[] updateOrder, string[] renderOrder)
		{
			this.updateOrder = updateOrder;
			this.renderOrder = renderOrder;

			EntityMap = new EntityMap();
		}

		public EntityMap EntityMap { get; }

		public void Add(string entityGroup, Entity entity)
		{
			if (!EntityMap.ContainsKey(entityGroup))
			{
				EntityMap.Add(entityGroup, new List<Entity>
				{
					entity
				});

				return;
			}

			EntityMap[entityGroup].Add(entity);
		}

		public void Update(float dt)
		{
			foreach (string type in updateOrder)
			{
				List<Entity> entityList;

				if (EntityMap.TryGetValue(type, out entityList))
				{
					entityList.ForEach(entity => entity.Update(dt));
				}
			}
		}

		public void Render(SpriteBatch sb)
		{
			foreach (string type in renderOrder)
			{
				List<Entity> entityList;

				if (EntityMap.TryGetValue(type, out entityList))
				{
					entityList.ForEach(entity => entity.Render(sb));
				}
			}
		}
	}
}
