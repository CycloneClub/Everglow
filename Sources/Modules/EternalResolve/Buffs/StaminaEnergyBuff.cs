using Everglow.Commons.Templates.Weapons.StabbingSwords;

namespace Everglow.EternalResolve.Buffs;

public class StaminaEnergyBuff : ModBuff
{
	public override void SetStaticDefaults()
	{
		Main.buffNoSave[Type] = false;
	}

	public override void Update(Player player, ref int buffIndex)
	{
		player.GetModPlayer<StabbingSwordStaminaPlayer>().StaminaRecoveryBonus += 0.2f;
	}
}
