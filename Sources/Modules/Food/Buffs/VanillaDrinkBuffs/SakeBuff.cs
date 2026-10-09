namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class SakeBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("SakeBuff");
		// Description.SetDefault("短时间内减少18防御，大幅增加近战能力\n“纯度，太高了。”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense -= 18; // 减18防御
		player.GetCritChance(DamageClass.Melee) += 40; // 加40%暴击
		player.GetDamage(DamageClass.Melee) += 1.4f; // 加40%伤害
		player.GetAttackSpeed(DamageClass.Generic) += 0.4f; // 加40%攻速

	}
}

