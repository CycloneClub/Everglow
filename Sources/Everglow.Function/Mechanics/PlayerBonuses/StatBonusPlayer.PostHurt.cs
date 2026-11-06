namespace Everglow.Commons.Mechanics.PlayerBonuses;

public partial class StatBonusPlayer
{
	public bool OrangeIcecream;
	public bool OrangeJuice;

	public override void PostHurt(Player.HurtInfo info)
	{
		// Native Hurt has assigned the general hit's immunity duration before this hook.
		if (info.CooldownCounter == ImmunityCooldownID.General)
		{
			// ==================== Additive duration ====================
			if (OrangeIcecream)
			{
				Player.immuneTime += 30;
			}

			// ==================== Multiplicative duration ====================
			if (OrangeJuice)
			{
				Player.immuneTime *= 2;
			}
		}
	}
}
