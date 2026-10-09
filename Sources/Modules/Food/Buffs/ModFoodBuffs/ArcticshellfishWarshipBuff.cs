namespace Everglow.Food.Buffs.ModFoodBuffs;

public class ArcticshellfishWarshipBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("ArcticshellfishWarshipBuff");
		// Description.SetDefault("提升防御\n“美味的北极贝军舰”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 4; // 加4防御

	}
}

