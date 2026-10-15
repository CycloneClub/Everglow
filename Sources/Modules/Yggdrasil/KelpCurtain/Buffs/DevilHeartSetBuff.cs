namespace Everglow.Yggdrasil.KelpCurtain.Buffs;

public class DevilHeartSetBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetDamage<MagicDamageClass>() += 0.06f; // Increases magic damage by 6%
		player.GetCritChance<MagicDamageClass>() += 6; // Increases magic critical chance by 6 percentage points
		player.maxMinions += 1; // Increases the number of minions the player can summon by 1

		// Bans mana regeneration
		player.manaRegenBonus = -100;
		player.manaRegen = 0;
		player.manaRegenCount = 0;

		player.statDefense *= 1f - 0.15f; // Reduces defense by 15%
	}
}
