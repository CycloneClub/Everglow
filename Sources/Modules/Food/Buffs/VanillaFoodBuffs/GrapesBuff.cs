namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class GrapesBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("GrapesBuff");
		// Description.SetDefault("增加1召唤栏，幸运值加10%，减12防御\n“多子多福 ”");
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.maxMinions += 1; // 加1召唤栏
		player.luck *= 1.1f;
		player.statDefense -= 12; // 减12防御
	}
}

