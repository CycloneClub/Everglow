namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class PrismaticPunchBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("PrismaticPunchBuff");
		// Description.SetDefault("短时间内增加5召唤栏，5哨兵栏，增加50%召唤伤害，击退\n“高雅兴致”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.maxMinions += 5; // 加5召唤栏
		player.maxTurrets += 5; // 加5哨兵栏
		player.GetKnockback(DamageClass.Summon) *= 1.5f; // 击退加倍
		player.GetDamage(DamageClass.Summon) *= 1.5f; // 加50%伤害

	}
}

