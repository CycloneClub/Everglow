using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Utilities;

namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class GrilledSquirrelBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.jumpSpeedBoost += 2;
		player.jumpBoost = true;
		// Cap maximum fall speed at 75% of the normal baseline (7.5 px/tick).
		player.AddStat(StatBonus.MaxFallSpeedCap, 7.5f);
		player.extraFall += 30;
	}
}
