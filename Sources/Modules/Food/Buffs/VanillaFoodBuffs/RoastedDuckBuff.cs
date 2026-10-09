namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class RoastedDuckBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("RoastedDuckBuff");
		// Description.SetDefault("可以在水上行走，提升飞行能力\n“这是烤鸭”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.RoastedDuckBuff = true;
		FoodBuffModPlayer.WingTimeModifier += 0.1f;
		player.maxFallSpeed *= 0.5f;
		player.extraFall += 30;
		player.waterWalk = true;

	}
}

