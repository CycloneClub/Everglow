using Everglow.Commons.Utilities;

namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class B_52Buff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		if (ItemUtils.Ranged.Launchers.Contains(player.HeldItem.type))
		{
			// +100% ranged attack speed
			player.GetAttackSpeed(DamageClass.Ranged) += 1f;
			player.GetCritChance(DamageClass.Ranged) += 25;
			// +50% ranged damage
			player.GetDamage(DamageClass.Ranged) += 0.5f;
		}
	}
}
