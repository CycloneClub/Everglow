namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class SeafoodDinnerBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +6% critical strike chance
		player.GetCritChance(DamageClass.Generic) += 6;
		// +6% damage
		player.GetDamage(DamageClass.Generic) += 0.06f;
		// +6% attack speed
		player.GetAttackSpeed(DamageClass.Generic) += 0.06f;
		FoodBuffModPlayer.AddCritDamage += 0.06f;
	}
}
