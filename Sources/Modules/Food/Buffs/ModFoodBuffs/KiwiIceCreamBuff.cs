namespace Everglow.Food.Buffs.ModFoodBuffs;

public class KiwiIceCreamBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.KiwiIceCreamBuff = true;
		// +5% summon attack speed
		player.GetAttackSpeed(DamageClass.Summon) += 0.05f;
	}
}
