namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PhoBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("PhoBuff");
		// Description.SetDefault("增加8%召唤物伤害\n“异域风情 ”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetDamage(DamageClass.Summon) *= 1.08f;

	}
}

