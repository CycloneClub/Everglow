namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class GrapeJuiceBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("GrapeJuiceBuff");
		// Description.SetDefault("短时间极其幸运\n“浓缩的幸福”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.luck += 10000;
	}
}

