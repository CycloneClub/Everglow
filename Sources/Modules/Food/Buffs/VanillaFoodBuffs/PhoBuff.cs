namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class PhoBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +8% summon damage
		player.GetDamage(DamageClass.Summon) += 0.08f;
	}
}
