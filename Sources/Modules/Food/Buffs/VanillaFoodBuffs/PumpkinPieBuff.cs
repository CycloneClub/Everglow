namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PumpkinPieBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("PumpkinPieBuff");
		// Description.SetDefault("增加50最大生命值\n“丰收的喜悦”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.statLifeMax2 += 50;

	}
}

