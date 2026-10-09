namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class TeacupBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("TeacupBuff");
		// Description.SetDefault("短时间内大幅增加魔力回复，魔法伤害，暴击\n“醇香四溢”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetDamage(DamageClass.Magic) *= 1.6f; // 加60%攻击
		player.GetCritChance(DamageClass.Magic) += 60; // 加60%暴击
		player.manaRegen += 100; // 加100魔力回复
	}
}
