namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class AleBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		//DisplayName.SetDefault("AleBuff");
		//Description.SetDefault("短时间内鞭子的范围和速度提升至1.5倍\n“耍酒疯”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.whipRangeMultiplier *= 1.5f; //
		player.GetAttackSpeed(DamageClass.Summon) *= 1.5f;

	}
}

