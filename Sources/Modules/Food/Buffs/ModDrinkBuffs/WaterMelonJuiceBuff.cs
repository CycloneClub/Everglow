namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class WaterMelonJuiceBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("WaterMelonJuiceBuff");
		// Description.SetDefault("短时间大幅增加击退与暴击\n“浓缩的打击力度”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetCritChance(DamageClass.Generic) += 50;
		player.GetKnockback(DamageClass.Generic) *= 2f;
	}
}
