namespace Everglow.Yggdrasil;

public class YggdrasilWorldGlobalItem : GlobalItem
{
	public override bool CanUseItem(Item item, Player player)
	{
		return !YggdrasilWorld.InYggdrasil || item.type is not (ItemID.BloodMoonStarter or ItemID.SolarTablet or ItemID.GoblinBattleStandard
			or ItemID.PirateMap or ItemID.SnowGlobe or ItemID.PumpkinMoonMedallion or ItemID.NaughtyPresent
			or ItemID.DD2ElderCrystal or ItemID.CelestialSigil);
	}

	public override bool ConsumeItem(Item item, Player player) => CanUseItem(item, player);
}
