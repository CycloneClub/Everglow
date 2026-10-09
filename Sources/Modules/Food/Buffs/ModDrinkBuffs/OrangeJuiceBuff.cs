namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class OrangeJuiceBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("OrangeJuiceBuff");
		// Description.SetDefault("短时间内获得超长无敌帧\n“嗯，有点像LCL”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.immuneTime *= 2;
	}
}

