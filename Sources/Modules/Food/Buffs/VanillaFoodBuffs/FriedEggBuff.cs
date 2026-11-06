namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class FriedEggBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.FriedEggBuff = true;
	}
}
