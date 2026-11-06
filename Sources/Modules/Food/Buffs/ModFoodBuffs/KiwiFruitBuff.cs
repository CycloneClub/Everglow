namespace Everglow.Food.Buffs.ModFoodBuffs;

public class KiwiFruitBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.KiwiFruitBuff = true;
	}
}
