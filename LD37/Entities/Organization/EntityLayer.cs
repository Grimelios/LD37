using System;
using System.Collections.Generic;
using LD37.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace LD37.Entities.Organization
{
	using EntityMap = Dictionary<Type, List<Entity>>;

	internal class EntityLayer : IDynamic, IRenderable
	{
		private Type[] updateOrder;
		private Type[] renderOrder;
		private EntityMap entityMap;

		public EntityLayer(Type[] updateOrder, Type[] renderOrder)
		{
			this.updateOrder = updateOrder;
			this.renderOrder = renderOrder;

			entityMap = new EntityMap();
		}

		public void Add(Type entityType, Entity entity)
		{
			if (!entityMap.ContainsKey(entityType))
			{
				entityMap.Add(entityType, new List<Entity>
				{
					entity
				});

				return;
			}

			entityMap[entityType].Add(entity);
		}

		public void Update(float dt)
		{
			foreach (Type type in updateOrder)
			{
				List<Entity> entityList;

				if (entityMap.TryGetValue(type, out entityList))
				{
					entityList.ForEach(entity => entity.Update(dt));
				}
			}
		}

		public void Render(SpriteBatch sb)
		{
			foreach (Type type in renderOrder)
			{
				List<Entity> entityList;

				if (entityMap.TryGetValue(type, out entityList))
				{
					entityList.ForEach(entity => entity.Render(sb));
				}
			}
		}
	}
}
