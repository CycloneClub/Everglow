namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class LemonBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetCritChance(DamageClass.Ranged) += 4; // 加4%暴击
		player.aggro -= 300; // 仇恨值减300
	}
}
