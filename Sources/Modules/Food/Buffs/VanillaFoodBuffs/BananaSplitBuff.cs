using Everglow.Commons.Utilities;

namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class BananaSplitBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.BananaSplitBuff = true;
		if (ItemUtils.Ranged.Shotguns.Contains(player.HeldItem.type))
		{
			player.GetCritChance(DamageClass.Ranged) += 6;
		}
	}
}
