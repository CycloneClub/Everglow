using Everglow.Commons.Utilities;

namespace Everglow.Food.Buffs.ModFoodBuffs;

public class MangosteenBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		if (ItemUtils.Melee.Boomerangs.Contains(player.HeldItem.type)
			|| ItemUtils.Melee.Flails.Contains(player.HeldItem.type)
			|| ItemUtils.Melee.Yoyos.Contains(player.HeldItem.type))
		{
			// +50% ranged attack speed
			player.GetAttackSpeed(DamageClass.Ranged) += 0.5f;
		}
	}
}
