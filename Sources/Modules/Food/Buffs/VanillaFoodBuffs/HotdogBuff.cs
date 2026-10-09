namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class HotdogBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("GrubSoupBuff");
		// Description.SetDefault("减少移速，增加近战伤害\n“吔~~”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.maxRunSpeed *= 0.8f;
		player.runAcceleration *= 0.8f;
		player.GetCritChance(DamageClass.Melee) += 8; // 加8%暴击
		player.GetDamage(DamageClass.Melee) *= 1.08f; // 加8%伤害

	}
}

