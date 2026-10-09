namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class GreenStormBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("GreenStormBuff");
		// Description.SetDefault("短时间交替射出叶绿水晶和孢子云\n“别以为草便宜,没上规模前,它比苹果核桃什么的都贵。”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.GreenStormBuff = true;
	}
}

