namespace Everglow.Food.Buffs.ModFoodBuffs;

public class KiwiFruitBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("KiwiFruitBuff");
		// Description.SetDefault("增大20%武器大小\n“又称奇异果”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.KiwiFruitBuff = true;
	}
}

