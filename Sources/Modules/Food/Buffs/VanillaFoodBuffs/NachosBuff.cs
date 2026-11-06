namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class NachosBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.NachosBuff = true;
	}
}
