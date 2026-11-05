namespace Everglow.Commons.Mechanics.PlayerBonuses;

public partial class StatBonusPlayer
{
	public bool OrangeCrown;
	public bool FragmentsOfWreckage;
	public bool WalnutClip;
	public bool FireflyBorageBadgeAgainstMoth;
	public bool YellowBellFlower;

	private float maxFallSpeedCap = float.PositiveInfinity;
	private float? maxFallSpeedForSpecialBehavior;

	public void Register(StatBonus target, float value)
	{
		if (target == StatBonus.MaxFallSpeedCap)
		{
			maxFallSpeedCap = Math.Min(maxFallSpeedCap, value);
		}
		else if (target == StatBonus.MaxFallSpeedForSpecialBehavior)
		{
			maxFallSpeedForSpecialBehavior = Math.Max(maxFallSpeedForSpecialBehavior ?? value, value);
		}
	}

	public override void PostUpdateEquips()
	{
		// ====================================================================
		// Life capacity: use completed maximum life; these effects may add defense.
		// ====================================================================
		if (OrangeCrown)
		{
			// +3 defense per 100 maximum life.
			Player.statDefense += (int)(Player.statLifeMax2 * 0.03f);
		}
		if (FragmentsOfWreckage)
		{
			// Scale all four bonuses from the same missing-life value, capped at 120.
			float scale = Math.Clamp((Player.statLifeMax2 - Player.statLife) / 120f, 0f, 1f);
			// Up to +8 defense, +6% damage, +6% melee speed and +10% mining speed.
			Player.statDefense += (int)(8 * scale);
			Player.GetDamage(DamageClass.Generic) += 0.06f * scale;
			Player.GetAttackSpeed(DamageClass.Melee) += 0.06f * scale;
			Player.pickSpeed -= 0.1f * scale;
		}
		if (WalnutClip && Player.statLifeMax2 / 2f > Player.statLife)
		{
			// +1% damage per 4 life missing below 50% maximum life.
			Player.GetDamage(DamageClass.Generic) += (Player.statLifeMax2 / 2f - Player.statLife) / 400f;
		}

		// ====================================================================
		// Defense: adjust collected defense before converting it into damage.
		// ====================================================================
		if (FireflyBorageBadgeAgainstMoth)
		{
			int penalty = Player.statDefense <= 50 ? Player.statDefense / 5 : 10;
			Player.statDefense -= penalty;
		}

		if (YellowBellFlower)
		{
			// +0.2% damage per defense.
			Player.GetDamage(DamageClass.Generic) += (int)Player.statDefense * 0.002f;
		}

		// ====================================================================
		// Fall speed: apply the independent cap or special-behavior override.
		// ====================================================================
		Player.maxFallSpeed = maxFallSpeedForSpecialBehavior ?? Math.Min(Player.maxFallSpeed, maxFallSpeedCap);
	}
}
