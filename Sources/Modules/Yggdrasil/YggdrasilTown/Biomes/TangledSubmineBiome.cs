using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Biomes;

public class TangledSubmineBiome : ModBiome
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Biomes;

	public override string BestiaryIcon => Commons.ModAsset.Cavern_Mod;

	public override string BackgroundPath => ModAsset.YggdrasilTown_MapBackground_Mod;

	public override int Music => -1;

	public override bool IsBiomeActive(Player player) =>
		YggdrasilWorld.InYggdrasil && YggdrasilTownGeneration.IsInTangledSubmine(player.Center.ToTileCoordinates());
}
