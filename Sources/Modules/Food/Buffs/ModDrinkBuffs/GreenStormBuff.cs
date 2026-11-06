namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class GreenStormBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.GreenStormBuff = true;
	}
}
