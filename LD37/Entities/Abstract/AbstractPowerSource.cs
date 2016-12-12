using System.Collections.Generic;
using LD37.Interfaces;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace LD37.Entities.Abstract
{
	internal abstract class AbstractPowerSource : Entity, IPowered
	{
		private static int nextID;

		public static int NextID => nextID++;

		private bool powered;

		private List<int> targetIDs;
		private List<IPowered> powerTargets;
		private Wire wire;

		protected AbstractPowerSource()
		{
			targetIDs = new List<int>();
			powerTargets = new List<IPowered>();
			PowerID = NextID;
		}

		public Wire Wire
		{
			set
			{
				wire = value;
				wire.Powered = powered;
			}
		}

		[JsonProperty]
		public virtual bool Powered
		{
			get { return powered; }
			set
			{
				powered = value;

				if (wire != null)
				{
					wire.Powered = value;
				}

				if (powerTargets != null)
				{
					foreach (IPowered target in powerTargets)
					{
						target.Powered = value;
					}
				}
			}
		}

		[JsonProperty]
		public int PowerID { get; set; }

		[JsonProperty]
		public List<int> TargetIDs
		{
			get { return targetIDs; }
			set
			{
				if (value != null)
				{
					targetIDs = value;
				}
			}
		}

		[JsonIgnore]
		public List<IPowered> PowerTargets
		{
			get { return powerTargets; }
			set
			{
				powerTargets = value;

				foreach (IPowered target in powerTargets)
				{
					target.Powered = powered;
				}
			}
		}

		[JsonIgnore]
		public abstract Vector2 WirePosition { get; }
	}
}
