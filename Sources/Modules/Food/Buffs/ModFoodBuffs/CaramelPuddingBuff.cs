namespace Everglow.Food.Buffs.ModFoodBuffs;

public class CaramelPuddingBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.CaramelPuddingBuff = true;
	}
}
