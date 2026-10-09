namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PineappleBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("PineappleBuff");
		// Description.SetDefault("增加4防御，近战攻击者伤害玩家时会承受一半伤害\n“菠萝碱”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 4; // 加4防御
		player.thorns += 0.5f; // 50%反伤
	}
}

