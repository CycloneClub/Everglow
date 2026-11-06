using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Utilities;

namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class RoastedDuckBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.RoastedDuckBuff = true;
		// Cap maximum fall speed at 50% of the normal baseline (5 px/tick).
		player.AddStat(StatBonus.MaxFallSpeedCap, 5f);
		player.extraFall += 30;
		player.waterWalk = true;
	}
}
