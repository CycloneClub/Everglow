namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class RoastedBirdBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("RoastedBirdBuff");
		// Description.SetDefault("提升飞行能力\n“我是一只小小小鸟”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.RoastedBirdBuff = true;
		FoodBuffModPlayer.WingTimeModifier += 0.25f;
		player.extraFall += 30;
		player.wingAccRunSpeed *= 1.15f;

	}
}

