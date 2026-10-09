namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class BlueHawaiiBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("BlueHawaiiBuff");
		// Description.SetDefault("短时间内射弹速度获得提升\n“海风吹拂”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.BlueHawaiiBuff = true;
	}
}
