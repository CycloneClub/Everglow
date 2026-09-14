using Everglow.Yggdrasil.KelpCurtain.Tiles;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Placeables;

public class AlgaeExtractor_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<AlgaeExtractor>());
		Item.width = 16;
		Item.height = 16;
	}
}
