using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Utilities;

namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class GrilledSquirrelBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.jumpSpeedBoost += 2;
		player.jumpBoost = true;
		// Cap maximum fall speed at 75% of the normal baseline (7.5 px/tick).
		player.AddStat(StatBonus.MaxFallSpeedCap, 7.5f);
		player.extraFall += 30;
	}
}
