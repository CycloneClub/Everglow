namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PlumBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.aggro += 600; // 加600仇恨值
		player.GetAttackSpeed(DamageClass.Generic) += 0.08f; // 加8%攻速
	}
}
