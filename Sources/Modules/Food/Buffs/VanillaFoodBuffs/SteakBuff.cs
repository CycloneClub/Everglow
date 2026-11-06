namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class SteakBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// -20% mana cost
		player.manaCost -= 0.20f;
	}
}
