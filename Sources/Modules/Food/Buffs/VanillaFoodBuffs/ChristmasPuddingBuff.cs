namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ChristmasPuddingBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("ChristmasPuddingBuff");
		// Description.SetDefault("仇恨值减少800\n“美容养颜”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.aggro -= 800; // 仇恨值减800

	}
}

