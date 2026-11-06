namespace Everglow.Food.Buffs.ModFoodBuffs;

public class WatermelonBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +8% knockback
		player.GetKnockback(DamageClass.Generic) += 0.08f;
	}
}
