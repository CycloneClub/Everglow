namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class KiwiJuiceBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("KiwiJuiceBuff");
		// Description.SetDefault("短时间内大幅增大武器大小\n“浓缩的奇异之力”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.KiwiJuiceBuff = true;
	}
}

