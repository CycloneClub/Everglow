namespace Everglow.Yggdrasil.Mechanics.TownCrowd;

/// <summary>Separate safety recovery from continuous offscreen retirement.</summary>
public sealed class CrowdAvoidance
{
	private int safeTicks;
	private int offscreenTicks;
	private bool retiring;

	public bool Alarmed { get; private set; }

	public bool PendingRetirement => retiring;

	public bool Update(bool threatened, bool safe, bool visible, int retirementTicks)
	{
		if (threatened)
		{
			Alarmed = true;
			retiring = true;
			safeTicks = 0;
		}
		else if (Alarmed)
		{
			safeTicks = safe ? safeTicks + 1 : 0;
			if (safeTicks >= 180)
			{
				Alarmed = false;
			}
		}
		if (visible)
		{
			offscreenTicks = 0;
			if (!Alarmed)
			{
				retiring = false;
			}
		}
		else if (retiring)
		{
			offscreenTicks++;
		}
		return retiring && offscreenTicks >= retirementTicks;
	}
}
