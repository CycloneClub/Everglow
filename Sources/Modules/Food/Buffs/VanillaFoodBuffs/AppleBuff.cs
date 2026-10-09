namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class AppleBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("AppleBuff");
		// Description.SetDefault("增加5%减伤\n“一天一苹果，医生远离我”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.endurance += 0.05f; // 加8%减伤
	}
}

