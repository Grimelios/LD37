namespace LD37.Interfaces
{
	internal interface IPowerSource : IPowered
	{
		int[] TargetIDs { get; set; }

		IPowered[] PowerTargets { set; }
	}
}
