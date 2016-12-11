using LD37.Interfaces;
using Newtonsoft.Json;

namespace LD37.Entities.Abstract
{
	internal abstract class AbstractPowerSource : Entity, IPowered
	{
		private bool powered;

		private IPowered[] powerTargets;

		protected AbstractPowerSource()
		{
		}

		[JsonProperty]
		public virtual bool Powered
		{
			get { return powered; }
			set
			{
				powered = value;

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
		public int[] TargetIDs { get; set; }

		public IPowered[] PowerTargets
		{
			set
			{
				powerTargets = value;

				foreach (IPowered target in powerTargets)
				{
					target.Powered = powered;
				}
			}
		}
	}
}
