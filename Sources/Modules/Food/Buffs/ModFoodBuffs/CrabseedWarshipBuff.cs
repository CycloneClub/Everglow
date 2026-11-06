namespace Everglow.Food.Buffs.ModFoodBuffs;

public class CrabseedWarshipBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetCritChance(DamageClass.Generic) += 4; // 加4暴击
	}
}
