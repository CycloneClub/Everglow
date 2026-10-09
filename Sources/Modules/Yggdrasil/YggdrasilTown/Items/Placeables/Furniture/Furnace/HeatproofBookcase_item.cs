using Terraria.GameContent.Creative;
using Everglow.Commons.Utilities;
using Everglow.Commons.Templates.Furniture;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.Furnace;

public class HeatproofBookcase_item : BookcaseItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.FurnaceTiles.HeatproofBookcase>());
		base.SetDefaults();
	}
}
