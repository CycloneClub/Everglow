namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class QuinceMarryBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("QuinceMarryBuff");
		// Description.SetDefault("对南瓜月敌怪特攻\n“炙热与温柔”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.QuinceMarryBuff = true;

	}
}

