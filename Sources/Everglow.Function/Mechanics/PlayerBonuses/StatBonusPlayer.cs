namespace Everglow.Commons.Mechanics.PlayerBonuses;

public class StatBonusPlayer : ModPlayer
{
	private float maxFallSpeedCap = float.PositiveInfinity;
	private float? maxFallSpeedForSpecialBehavior;

	public override void ResetEffects()
	{
		maxFallSpeedCap = float.PositiveInfinity;
		maxFallSpeedForSpecialBehavior = null;
	}

	public void Register(StatBonus target, float value)
	{
		switch (target)
		{
			case StatBonus.MaxFallSpeedCap:
				maxFallSpeedCap = Math.Min(maxFallSpeedCap, value);
				break;
			case StatBonus.MaxFallSpeedForSpecialBehavior:
				maxFallSpeedForSpecialBehavior = Math.Max(maxFallSpeedForSpecialBehavior ?? value, value);
				break;
		}
	}

	public override void PostUpdateEquips()
	{
		Player.maxFallSpeed = maxFallSpeedForSpecialBehavior ?? Math.Min(Player.maxFallSpeed, maxFallSpeedCap);
	}
}
