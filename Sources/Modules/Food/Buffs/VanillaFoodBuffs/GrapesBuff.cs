namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class GrapesBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoTimeDisplay[Type] = false;
		Main.debuff[Type] = false; // 添加这个，这样护士在治疗时就不会去除buff
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.maxMinions += 1; // 加1召唤栏
		// +0.1 luck
		player.equipmentBasedLuckBonus += 0.1f;
		player.statDefense -= 12; // 减12防御
	}
}
