namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class CreamSodaBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("CreamSodaBuff");
		// Description.SetDefault("短时间内大幅增加近战能力，但极其吸引仇恨\n“喷射！”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.aggro -= 2400;
		player.GetCritChance(DamageClass.Melee) += 40; // 加40%暴击
		player.GetDamage(DamageClass.Melee) *= 1.4f; // 加40%伤害
		player.GetAttackSpeed(DamageClass.Generic) += 0.4f; // 加40%攻速
	}
}

