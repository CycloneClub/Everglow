namespace Everglow.Food.Buffs.ModFoodBuffs;

public class StrawberryBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.StrawberryBuff = true;
	}
}
