namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class SmoothieofDarknessBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("SmoothieofDarknessBuff");
		// Description.SetDefault("短时间内有50%的概率闪避攻击\n“腹黑（字面意义上）”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.SmoothieofDarknessBuff = true;

	}
}

