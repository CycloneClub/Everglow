namespace Everglow.Food.Buffs.ModFoodBuffs;

public class SeafoodPizzaBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetCritChance(DamageClass.Generic) += 8; // 加8%暴击
		// +8% damage
		player.GetDamage(DamageClass.Generic) += 0.08f;
		// +8% attack speed
		player.GetAttackSpeed(DamageClass.Generic) += 0.08f;
		FoodBuffModPlayer.AddCritDamage += 0.08f;
		player.statDefense += 2; // 加2防御
		player.lifeRegen += 1; // 加1生命回复
		// +1 mana regen
		player.manaRegenBonus += 1;
	}
}
