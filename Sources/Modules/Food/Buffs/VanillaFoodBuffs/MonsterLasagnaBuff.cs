using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Mechanics;

namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class MonsterLasagnaBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetModPlayer<StatBonusPlayer>().MonsterLasagna = true;
		player.GetCritChance(DamageClass.Generic) += 25;
	}
}
