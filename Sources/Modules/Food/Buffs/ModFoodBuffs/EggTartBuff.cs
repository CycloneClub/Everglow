namespace Everglow.Food.Buffs.ModFoodBuffs;

public class EggTartBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer.AddCritDamage += 1.05f;
	}
}
