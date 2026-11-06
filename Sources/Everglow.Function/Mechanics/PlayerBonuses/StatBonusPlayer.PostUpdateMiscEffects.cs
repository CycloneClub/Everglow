using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.PlayerBonuses;

public partial class StatBonusPlayer
{
	public bool BloodTearCrystalCrown;
	public bool WhiteCrown;
	public bool CelesteStoneWaistPendant;
	public bool DevilHeartSet;
	public bool ThermalConductor;

	public const int ThermalConductorCheckFrames = 60;
	public const float ThermalConductorManaThreshold = 0.2f;
	public const float ThermalConductorTriggerRate = 0.25f;
	public const int ThermalConductorDebuffDuration = 120;
	public const int ThermalConductorManaHeal = 36;

	public override void PostUpdateMiscEffects()
	{
		// ====================================================================
		// Life capacity: evaluate life thresholds and movement bonuses.
		// ====================================================================
		if (BloodTearCrystalCrown && Player.statLife > Player.statLifeMax2 * 0.75f)
		{
			// +1 life regen above 75% maximum life.
			Player.lifeRegen += 1;
		}
		if (WhiteCrown)
		{
			// +10% movement speed per 50 maximum life.
			Player.moveSpeed += (int)(Player.statLifeMax2 * 0.02f) / 10f;
		}

		// ====================================================================
		// Mana capacity: collect natural-regen bonuses.
		// ====================================================================
		if (CelesteStoneWaistPendant && Player.statMana > Player.statManaMax2 * 0.3f && Player.statMana < Player.statManaMax2 * 0.7f)
		{
			Player.manaRegenBonus += 3;
		}

		// ====================================================================
		// Natural regen: apply suppression after collecting bonuses.
		// ====================================================================
		if (DevilHeartSet)
		{
			// Native regen consumes at most 2.05 + manaRegenDelayBonus this tick.
			// Preserve additive bonuses and longer delays while suppressing native regen.
			Player.manaRegenDelay = Math.Max(Player.manaRegenDelay, Player.manaRegenDelayBonus + 3f);
			Player.manaRegen = 0;
			Player.manaRegenCount = 0;
		}

		// ====================================================================
		// Direct recovery: restore mana independently of natural regen.
		// ====================================================================
		if (ThermalConductor && Main.GameUpdateCount % ThermalConductorCheckFrames == 0
			&& Player.statMana < ThermalConductorManaThreshold * Player.statManaMax2
			&& Main.rand.NextFloat() < ThermalConductorTriggerRate)
		{
			Player.HealMana(ThermalConductorManaHeal);
			Player.AddBuff(BuffID.CursedInferno, ThermalConductorDebuffDuration);
		}
	}
}
