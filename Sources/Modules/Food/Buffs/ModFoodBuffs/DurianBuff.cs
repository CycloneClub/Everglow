namespace Everglow.Food.Buffs.ModFoodBuffs;

public class DurianBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.DurianBuff = true;
	}
}
