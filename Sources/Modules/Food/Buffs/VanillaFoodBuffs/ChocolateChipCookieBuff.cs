namespace Everglow.Food.Buffs.VanillaFoodBuffs;

public class ChocolateChipCookieBuff : ModBuff
{
	public override void Update(Player player, ref int buffIndex)
	{
		player.lifeRegen += 2; // 加2生命恢复
		// +2 mana regen
		player.manaRegenBonus += 2;
	}
}
