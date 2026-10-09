namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class FriedEggBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("FriedEggBuff");
		// Description.SetDefault("你可以更远地扔出投掷类武器\n“蛋白质”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.FriedEggBuff = true;
	}
}

