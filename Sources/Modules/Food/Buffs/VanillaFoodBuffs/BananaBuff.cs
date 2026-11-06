using Everglow.Commons.Utilities;

namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class BananaBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.BananaBuff = true;
		if (ItemUtils.Ranged.Shotguns.Contains(player.HeldItem.type))
		{
			player.GetAttackSpeed(DamageClass.Ranged) += 0.05f;
		}
	}
}
