namespace Everglow.Food.Buffs.ModDrinkBuffs;

public class WaterMelonJuiceBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.GetCritChance(DamageClass.Generic) += 50;
		// +100% knockback
		player.GetKnockback(DamageClass.Generic) += 1f;
	}
}
