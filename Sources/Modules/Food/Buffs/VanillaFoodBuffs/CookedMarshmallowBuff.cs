using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Utilities;

namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class CookedMarshmallowBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		// Cap maximum fall speed at 25% of the normal baseline (2.5 px/tick).
		player.AddStat(StatBonus.MaxFallSpeedCap, 2.5f);
		player.noFallDmg = true;
	}
}
