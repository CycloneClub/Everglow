namespace Everglow.Food.Buffs.ModFoodBuffs;

public class WakameWarshipBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("WakameWarshipBuff");
		// Description.SetDefault("提升魔力回复\n“美味的裙带菜寿司”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.manaRegen += 2;

	}
}

