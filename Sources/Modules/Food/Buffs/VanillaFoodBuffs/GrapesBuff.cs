namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class GrapesBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.maxMinions += 1; // 加1召唤栏
		// +0.1 luck
		player.equipmentBasedLuckBonus += 0.1f;
		player.statDefense -= 12; // 减12防御
	}
}
