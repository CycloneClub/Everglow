namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class CherryBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("CherryBuff");
		// Description.SetDefault("死亡后产生爆炸\n“你所热爱的，就是你的生活”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.CherryBuff = true;
	}
}

