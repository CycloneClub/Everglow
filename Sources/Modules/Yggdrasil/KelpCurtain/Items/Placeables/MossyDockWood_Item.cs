using Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Placeables;

public class MossyDockWood_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<MossyDockWood>());
	}
}
