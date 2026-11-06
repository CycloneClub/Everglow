namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class PiercoldWindBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.PiercoldWindBuff = true;
	}
}
