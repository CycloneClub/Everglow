namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class BurgerBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("BurgerBuff");
		// Description.SetDefault("减少移速，增加防御\n“热量炸弹”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.maxRunSpeed *= 0.6f;
		player.runAcceleration *= 0.6f;
		player.statDefense += 10;
	}
}
