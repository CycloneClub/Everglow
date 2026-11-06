namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class BowlofSoupBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +5% magic damage
		player.GetDamage(DamageClass.Magic) += 0.05f;
		player.statManaMax2 += 20; // +20 mana
	}
}
