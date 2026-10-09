namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class PurpleHooterBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("PurpleHooterBuff");
		// Description.SetDefault("对血月敌怪特攻\n“高贵与深沉”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.PurpleHooterBuff = true;
	}
}

