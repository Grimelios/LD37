using System.Collections.Generic;
using LD37.Entities.Abstract;
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

			HashSet<string> typeSet = new HashSet<string>(updateOrder);
			typeSet.UnionWith(renderOrder);

			foreach (string type in typeSet)
			{
				EntityMap.Add(type, new List<Entity>());
			}
		}

		public EntityMap EntityMap { get; }

		public void Update(float dt)
		{
			foreach (string type in updateOrder)
			{
				EntityMap[type].ForEach(entity => entity.Update(dt));
			}
		}

		public void Render(SpriteBatch sb)
		{
			foreach (string type in renderOrder)
			{
				EntityMap[type].ForEach(entity => entity.Render(sb));
			}
		}
	}
}
