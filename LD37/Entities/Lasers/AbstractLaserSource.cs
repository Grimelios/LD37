using System.Collections.Generic;
using LD37.Entities.Organization;
using LD37.Interfaces;
using LD37.Physics;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace LD37.Entities.Lasers
{
	using EntityMap = Dictionary<string, List<Entity>>;

	internal abstract class AbstractLaserSource : Entity, IPowered
	{
		private Laser laser;
		private EntityMap entityMap;

		private bool powered;

		protected AbstractLaserSource(PhysicsHelper physicsHelper, PrimitiveDrawer primitiveDrawer, Scene scene)
		{
			entityMap = scene.LayerMap["Primary"].EntityMap;
			laser = new Laser(physicsHelper, primitiveDrawer);
		}

		public override Vector2 Position
		{
			set
			{
				if (powered)
				{
					laser.RecomputePoints(value, Rotation);
				}

				base.Position = value;
			}
		}
		
		public virtual Color Color
		{
			set { laser.Color = value; }
		}

		public override float Rotation
		{
			set
			{
				if (powered)
				{
					laser.RecomputePoints(Position, value);
				}

				base.Rotation = value;
			}
		}

		[JsonProperty]
		public string ColorString
		{
			set
			{
				string[] tokens = value.Split(',');

				int r = int.Parse(tokens[0]);
				int g = int.Parse(tokens[1]);
				int b = int.Parse(tokens[2]);

				Color = new Color(r, g, b);
			}
		}

		[JsonProperty(Order = 1)]
		public bool Powered
		{
			get { return powered; }
			set
			{
				powered = value;

				if (powered)
				{
					laser.RecomputePoints(Position, Rotation);
					entityMap["Laser"].Add(laser);
				}
				else
				{
					entityMap["Laser"].Remove(laser);
				}
			}
		}

		public override void Dispose()
		{
			if (powered)
			{
				Powered = false;
			}
		}
	}
}
