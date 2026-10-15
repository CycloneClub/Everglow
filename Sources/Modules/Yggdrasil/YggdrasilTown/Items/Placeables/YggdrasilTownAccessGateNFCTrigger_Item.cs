using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

public class YggdrasilTownAccessGateNFCTrigger_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<YggdrasilTownAccessGateNFCTrigger>());
		Item.width = 16;
		Item.height = 16;
	}
}
