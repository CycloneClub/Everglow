namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class GrapefruitBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("GrapefruitBuff");
		// Description.SetDefault("加50%召唤物击退\n “拒绝肾透支”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetKnockback(DamageClass.Summon) += 0.25f; // 加25%召唤物击退
	}
}

