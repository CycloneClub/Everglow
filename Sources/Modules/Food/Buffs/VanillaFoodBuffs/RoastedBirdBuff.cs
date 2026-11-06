namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class RoastedBirdBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.RoastedBirdBuff = true;
		player.extraFall += 30;
	}
}
