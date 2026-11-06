namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class CookedFishBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +4% magic critical strike chance
		player.GetCritChance(DamageClass.Magic) += 4f;
		// +40 mana
		player.statManaMax2 += 40;
	}
}
