namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ApricotBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +4 mana regen
		player.manaRegenBonus += 4;
	}
}
