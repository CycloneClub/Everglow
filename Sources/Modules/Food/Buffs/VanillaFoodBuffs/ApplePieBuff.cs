namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ApplePieBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("ApplePieBuff");
		// Description.SetDefault("加5%减伤,1生命回复\n“一天一苹果，医生远离我”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.endurance += 0.05f; // 加8%减伤
		player.lifeRegen += 1;
	}
}

