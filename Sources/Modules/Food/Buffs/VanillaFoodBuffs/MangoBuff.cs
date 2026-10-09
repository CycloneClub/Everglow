namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class MangoBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("MangoBuff");
		// Description.SetDefault("减缓因食物中毒而产生的持续减血效果\n“清胃解毒”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.MangoBuff = true;
	}
}
