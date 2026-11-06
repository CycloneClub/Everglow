using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Mechanics;

namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class MonsterLasagnaBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetModPlayer<StatBonusPlayer>().MonsterLasagna = true;
		player.GetCritChance(DamageClass.Generic) += 25;
	}
}
