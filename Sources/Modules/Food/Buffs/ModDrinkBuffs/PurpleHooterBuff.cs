namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class PurpleHooterBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.PurpleHooterBuff = true;
	}
}
