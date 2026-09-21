namespace Everglow.Yggdrasil.KelpCurtain.Buffs;

public sealed class RedAlgaeToxinState
{
	private int appliedDuration;

	public int Apply(bool armorSet)
	{
		appliedDuration = RedAlgae_FriendlyDebuff.Duration * (armorSet ? 2 : 1);
		return appliedDuration;
	}

	public int Consume(int remainingTime, bool armorSet)
	{
		int elapsed = Math.Clamp(appliedDuration - remainingTime, 0, appliedDuration);
		appliedDuration = 0;
		return elapsed > 10 ? (int)(elapsed * (armorSet ? 2.5f : 1f)) : 0;
	}
}

public interface IRedAlgaeToxinProjectile
{
}
