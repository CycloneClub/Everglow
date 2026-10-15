using Everglow.Yggdrasil.YggdrasilTown.Tiles.Pylons;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Pylons;

public class UnregisteredPylon_Item : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<UnregisteredPylon>());
	}
}
