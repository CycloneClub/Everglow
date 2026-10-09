namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class DragonfruitBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("DragonfruitBuff");
		// Description.SetDefault("攻击造成着火以及涂油\n“红红火火恍恍惚惚”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.DragonfruitBuff = true;

	}
}

