namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class GrubSoupBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("GrubSoupBuff");
		// Description.SetDefault("加20渔力，每秒减2生命\n“吃啥补啥，就是有点恶心”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.fishingSkill += 30;
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.GrubSoupBuff = true;
	}
}
