using Everglow.CagedDomain.Tiles.OnWallTiles;

namespace Everglow.CagedDomain.Items;

public class MorphoFrame_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<MorphoFrame>());
		Item.width = 26;
		Item.height = 26;
	}
}
