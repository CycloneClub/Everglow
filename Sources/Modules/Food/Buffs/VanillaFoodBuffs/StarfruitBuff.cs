namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class StarfruitBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("StarfruitBuff");
		// Description.SetDefault("使用霰弹枪类武器时会多射出一发子弹\n“1437大帝的淫威”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.StarfruitBuff = true;

	}
}

