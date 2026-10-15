using Everglow.Yggdrasil.YggdrasilTown.Tiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

public class ArrowSign_Item_LeftDown : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<ArrowSigns>());
		Item.value = 200;
		Item.placeStyle = 7;
		Item.width = 18;
		Item.height = 28;
	}
}
