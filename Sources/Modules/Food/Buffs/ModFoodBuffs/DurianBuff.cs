namespace Everglow.Food.Buffs.ModFoodBuffs;

public class DurianBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("DurianBuff");
		// Description.SetDefault("加快消化食物的速度\n“促进消化”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.DurianBuff = true;

	}
}

