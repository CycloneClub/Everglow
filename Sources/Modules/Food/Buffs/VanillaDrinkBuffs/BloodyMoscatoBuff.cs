namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class BloodyMoscatoBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.BloodyMoscatoBuff = true;
	}
}
