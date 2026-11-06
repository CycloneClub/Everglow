namespace Everglow.Food.Buffs.ModFoodBuffs;

public class CantaloupeJellyBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.CantaloupeJellyBuff = true;
	}
}
