using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Utilities;

namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class CookedMarshmallowBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// Cap maximum fall speed at 25% of the normal baseline (2.5 px/tick).
		player.AddStat(StatBonus.MaxFallSpeedCap, 2.5f);
		player.noFallDmg = true;
	}
}
