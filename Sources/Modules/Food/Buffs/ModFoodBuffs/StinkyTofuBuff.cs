namespace Everglow.Food.Buffs.ModFoodBuffs;

public class StinkyTofuBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("StinkyTofuBuff");
		// Description.SetDefault("使靠近的敌怪混乱\n“又香又臭”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.StinkyTofuBuff = true;
	}
}

