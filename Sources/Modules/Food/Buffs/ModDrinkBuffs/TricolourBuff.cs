namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class TricolourBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("TricolourBuff");
		// Description.SetDefault("对boss仆从特攻\n“复杂与专一”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		FoodBuffModPlayer FoodBuffModPlayer = player.GetModPlayer<FoodBuffModPlayer>();
		FoodBuffModPlayer.TricolourBuff = true;
	}
}
