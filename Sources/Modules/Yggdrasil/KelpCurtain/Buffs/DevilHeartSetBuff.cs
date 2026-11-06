using Everglow.Commons.Mechanics.PlayerBonuses;

namespace Everglow.Yggdrasil.KelpCurtain.Buffs;

public class DevilHeartSetBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetDamage<MagicDamageClass>() += 0.06f; // +6% magic damage
		player.GetCritChance<MagicDamageClass>() += 6; // +6% magic critical strike chance
		player.maxMinions += 1; // +1 minion slot

		// Disables natural mana regen
		player.GetModPlayer<StatBonusPlayer>().DevilHeartSet = true;

		player.statDefense *= 1f - 0.15f; // -15% defense
	}
}
