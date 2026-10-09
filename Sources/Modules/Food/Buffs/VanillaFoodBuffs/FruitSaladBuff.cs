namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class FruitSaladBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("FruitSaladBuff");
		// Description.SetDefault("提升大部分属性\n“长久的维生素！”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 3; // 加3防御
		player.GetCritChance(DamageClass.Generic) += 3; // 加3%暴击
		player.GetDamage(DamageClass.Generic) *= 1.03f; // 加3%伤害
		player.GetAttackSpeed(DamageClass.Generic) += 0.03f; // 加3%攻速
		player.lifeRegen += 2; // 加2生命回复
		player.manaRegen += 3; // 魔力再生加3
		player.maxRunSpeed *= 1.2f; // 加速
		player.runAcceleration *= 1.2f;
		player.jumpSpeedBoost += 1.5f;

	}
}

