namespace Everglow.Food.Buffs.VanillaDrinkBuffs;

public class TropicalSmoothieBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// -50% mana cost
		player.manaCost -= 0.5f;
		// +50% magic damage
		player.GetDamage(DamageClass.Magic) += 0.5f;
		// +50% magic critical strike chance
		player.GetCritChance(DamageClass.Magic) += 50;
	}
}
