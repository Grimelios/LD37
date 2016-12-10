using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LD37.Entities;
using Microsoft.Xna.Framework;

namespace LD37.Physics
{
	internal class RayCastResults
	{
		public RayCastResults(Vector2 position, Entity entity)
		{
			Position = position;
			Entity = entity;
		}

		public Vector2 Position { get; }
		public Entity Entity { get; }
	}
}
