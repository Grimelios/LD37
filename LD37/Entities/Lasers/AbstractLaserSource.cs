﻿using System.Collections.Generic;
using LD37.Entities.Abstract;
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
		
		public virtual Color Color
		{
			set { laser.Color = value; }
		}

		[JsonProperty]
		public string Tint
		{
			set { Color = GameFunctions.ParseColor(value); }
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
					entityMap["Laser"].Add(laser);
				}
				else
				{
					laser.Unpower();
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

		public override void Update(float dt)
		{
			if (Powered)
			{
				laser.Recast(Position, Rotation);
			}
		}
	}
}
