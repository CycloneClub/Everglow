namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class SunriseBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("SunriseBuff");
		// Description.SetDefault("对日食敌怪特攻\n“阳光与明媚”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.SunriseBuff = true;
	}
}
