namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class MilkshakeBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("MilkshakeBuff");
		// Description.SetDefault("短时间内大幅加移速\n“节食减肥 ”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.maxRunSpeed *= 5f;
		player.runAcceleration *= 5f;
	}
}
