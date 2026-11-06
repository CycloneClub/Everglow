namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class StarfruitBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.StarfruitBuff = true;
	}
}
