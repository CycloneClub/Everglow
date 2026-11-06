namespace Everglow.Food.Buffs.ModFoodBuffs;

public class WakameWarshipBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		// +2 mana regen
		player.manaRegenBonus += 2;
	}
}
