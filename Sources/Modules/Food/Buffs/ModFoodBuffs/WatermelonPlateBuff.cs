namespace Everglow.Food.Buffs.ModFoodBuffs;

public class WatermelonPlateBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetCritChance(DamageClass.Generic) += 6;
		FoodBuffModPlayer.AddCritDamage += 1.06f;
	}
}
