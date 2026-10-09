namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class FruitJuiceBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("FruitJuiceBuff");
		// Description.SetDefault("短时间内幅大大提升大部分属性\n“维生素！”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 20; // 加20防御
		player.GetCritChance(DamageClass.Generic) += 20; // 加20%暴击
		player.endurance += 0.25f; // 加25%减伤
		player.GetDamage(DamageClass.Generic) *= 1.2f; // 加20%伤害
		player.GetAttackSpeed(DamageClass.Generic) += 0.2f; // 加20%攻速
		player.lifeRegen += 10; // 加1生命回复
		player.manaRegen += 20; // 魔力再生加2
		player.maxRunSpeed *= 2f; // 加速
		player.runAcceleration *= 2f;
		player.jumpSpeedBoost += 2;
	}
}
