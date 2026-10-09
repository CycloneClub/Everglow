namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class DreamYearningBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("DreamYearningBuff");
		// Description.SetDefault("短时间内射弹速度获得提升\n“思君忆君，魂牵梦萦，翠销香暖云屏，更哪堪酒醒。”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.DreamYearningBuff = true;
	}
}

