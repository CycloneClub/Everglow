namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class RedWineBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.RedWineBuff = true;
	}
}
