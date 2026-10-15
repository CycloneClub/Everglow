namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury;

public sealed class ArmOfGiantTreeAttackState
{
	public enum AttackPhase
	{
		Idle,
		Charging,
		Smashing,
		Recovering,
		NormalSwing,
	}

	public const int ChargeDuration = 150;
	public const int SmashDuration = 45;
	public const int RecoveryDuration = 30;
	public const int NormalDuration = 24;
	public const int ImpactFrame = 30;

	public AttackPhase Phase { get; private set; }

	public int ChargeFrames { get; private set; }

	public int Timer { get; private set; }

	public float DamageMultiplier => Phase == AttackPhase.NormalSwing ? 1f : 0.75f + 1.25f * ChargeFrames / ChargeDuration;

	private bool shockwaveConsumed;

	public bool Begin(bool alternate)
	{
		if (Phase != AttackPhase.Idle)
		{
			return false;
		}
		Phase = alternate ? AttackPhase.NormalSwing : AttackPhase.Charging;
		ChargeFrames = Timer = 0;
		shockwaveConsumed = false;
		return true;
	}

	public bool Release()
	{
		if (Phase != AttackPhase.Charging)
		{
			return false;
		}
		Phase = AttackPhase.Smashing;
		Timer = 0;
		return true;
	}

	public void Tick(bool holdingSameItem)
	{
		if (!holdingSameItem)
		{
			Phase = AttackPhase.Idle;
			ChargeFrames = Timer = 0;
			return;
		}
		switch (Phase)
		{
			case AttackPhase.Charging:
				ChargeFrames = Math.Min(ChargeDuration, ChargeFrames + 1);
				break;
			case AttackPhase.Smashing:
				if (++Timer >= SmashDuration)
				{
					Phase = AttackPhase.Recovering;
					Timer = 0;
				}
				break;
			case AttackPhase.Recovering:
				if (++Timer >= RecoveryDuration)
				{
					Phase = AttackPhase.Idle;
				}
				break;
			case AttackPhase.NormalSwing:
				if (++Timer >= NormalDuration)
				{
					Phase = AttackPhase.Idle;
				}
				break;
		}
	}

	public bool ConsumeShockwave()
	{
		if (Phase != AttackPhase.Smashing || ChargeFrames < ChargeDuration || Timer < ImpactFrame || shockwaveConsumed)
		{
			return false;
		}
		shockwaveConsumed = true;
		return true;
	}

	public void Restore(AttackPhase phase, int charge, int timer)
	{
		Phase = phase;
		ChargeFrames = Math.Clamp(charge, 0, ChargeDuration);
		Timer = Math.Clamp(timer, 0, SmashDuration);
	}
}
