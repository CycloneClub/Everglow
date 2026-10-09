namespace Everglow.Food.Buffs.ModFoodBuffs;

public class WatermelonBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("WatermelonBuff");
		// Description.SetDefault("增强8%击退能力\n“最大的打击力度”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetKnockback(DamageClass.Generic) *= 1.08f;
	}
}
