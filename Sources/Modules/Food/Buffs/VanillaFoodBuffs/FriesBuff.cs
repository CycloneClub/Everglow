namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class FriesBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.statDefense += 4; // 加4防御
		player.GetCritChance(DamageClass.Generic) += 6; // 加6%暴击
	}
}
