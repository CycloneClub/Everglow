using Everglow.Yggdrasil.YggdrasilTown.Tiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

public class ManaSpringOre : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<ManaSpringOre_Tile>());
		Item.width = 16;
		Item.height = 16;
	}
}
