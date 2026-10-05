using Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

namespace Everglow.Yggdrasil.KelpCurtain;

public class QuetzalsWishPlayer : ModPlayer
{
	public int NextStage { get; private set; }

	private int comboTime;

	public int BeginAttack(bool alternate)
	{
		int stage = alternate ? 5 : NextStage;
		NextStage = alternate ? 0 : QuetzalsWishCombatRules.NextComboStage(NextStage);
		comboTime = 60;
		return stage;
	}

	public void ResetCombo()
	{
		NextStage = 0;
		comboTime = 0;
	}

	public override void PostUpdate()
	{
		if (Player.HeldItem.type != ModContent.ItemType<QuetzalsWish>() || Player.dead || --comboTime <= 0)
		{
			ResetCombo();
		}
	}

	public override void UpdateDead() => ResetCombo();
}
