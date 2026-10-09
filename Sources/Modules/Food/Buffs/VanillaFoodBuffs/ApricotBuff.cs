namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ApricotBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("ApricotBuff");
		// Description.SetDefault("增加加4魔力再生\n“止渴润肺”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.manaRegen += 4; // 魔力再生加4
	}
}

