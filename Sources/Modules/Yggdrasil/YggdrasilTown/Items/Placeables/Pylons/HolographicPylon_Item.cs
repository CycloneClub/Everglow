using Everglow.Yggdrasil.YggdrasilTown.Tiles.Pylons;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Pylons;

public class HolographicPylon_Item : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<HolographicPylon>());
	}
}
