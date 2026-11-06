namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class HotdogBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// -20% movement speed
		player.moveSpeed -= 0.2f;
		player.GetCritChance(DamageClass.Melee) += 8; // 加8%暴击
		// +8% melee damage
		player.GetDamage(DamageClass.Melee) += 0.08f;
	}
}
