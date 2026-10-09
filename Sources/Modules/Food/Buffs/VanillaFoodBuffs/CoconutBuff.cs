namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class CoconutBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("CoconutBuff");
		// Description.SetDefault("增加4防御，3%减伤\n“我从小啃到大”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 4; // 加4防御
		player.endurance += 0.03f; // 加3%减伤
	}
}

