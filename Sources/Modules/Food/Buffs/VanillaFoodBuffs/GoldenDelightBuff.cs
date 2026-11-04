namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class GoldenDelightBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.GoldenDelightBuff = true;
		player.statDefense += 4; // +4 defense
		player.GetCritChance(DamageClass.Generic) += 4; // 加4%暴击
		// +4% damage
		player.GetDamage(DamageClass.Generic) += 0.04f;
		player.GetAttackSpeed(DamageClass.Generic) += 0.04f; // 加4%攻速
		player.lifeRegen += 2; // 加2生命回复
		// +4 mana regen
		player.manaRegenBonus += 4;
		// +20% movement speed
		player.moveSpeed += 0.2f;
		player.jumpSpeedBoost += 2;
	}
}
