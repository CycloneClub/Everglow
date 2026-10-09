namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class RedWineBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("RedWineBuff");
		// Description.SetDefault("对霜月敌怪特攻\n“上流与优雅”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.RedWineBuff = true;
	}
}

