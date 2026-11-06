namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class DragonfruitBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.DragonfruitBuff = true;
	}
}
