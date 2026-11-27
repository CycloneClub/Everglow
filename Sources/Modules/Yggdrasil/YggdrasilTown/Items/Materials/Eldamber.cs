using EldamberTile = Everglow.Yggdrasil.YggdrasilTown.Tiles.Eldamber;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

public class Eldamber : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<EldamberTile>());
		Item.width = 26;
		Item.height = 30;
	}
}
