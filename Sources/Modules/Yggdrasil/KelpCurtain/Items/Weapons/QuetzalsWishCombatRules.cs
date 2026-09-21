namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public static class QuetzalsWishCombatRules
{
	public const int AttackFrames = 24;
	public const int ThrowChargeFrames = 120;
	public const int WoundFrames = 300;
	public const int ExplosionRadius = 96;

	public enum ChargeResult
	{
		Charging,
		Cancelled,
		Throw,
	}

	public static int NextComboStage(int stage) => (stage + 1) % 4;

	public static bool CanBranch(int stage) => stage is 2 or 3;

	public static ChargeResult ResolveCharge(int frames, bool held, bool ownerValid)
	{
		if (!ownerValid || !held)
		{
			return ChargeResult.Cancelled;
		}
		return frames >= ThrowChargeFrames ? ChargeResult.Throw : ChargeResult.Charging;
	}

	public static int WoundedLifeRegen(int lifeRegen, float velocityX, float velocityY)
	{
		return velocityX != 0f || velocityY != 0f ? Math.Min(0, lifeRegen) - 24 : lifeRegen;
	}

	public static bool ShouldExplode(int stage, bool impact) => stage == 6 && impact;
}
