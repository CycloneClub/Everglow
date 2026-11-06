namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class SugarCookieBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +5% ranged damage
		player.GetDamage(DamageClass.Ranged) += 0.05f;
	}
}
