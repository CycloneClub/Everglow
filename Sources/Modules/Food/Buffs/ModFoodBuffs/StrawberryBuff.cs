namespace Everglow.Food.Buffs.ModFoodBuffs;

public class StrawberryBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("StrawberryBuff");
		// Description.SetDefault("敌怪越近，造成的伤害越高\n“受人欢迎的水果”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.StrawberryBuff = true;
	}
}

