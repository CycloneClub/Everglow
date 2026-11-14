using Terraria.GameContent.Creative;
using Everglow.Commons.Utilities;
using Everglow.Commons.Templates.Furniture;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.Furnace;

public class HeatproofWorkbench_Item : WorkBenchItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.FurnaceTiles.HeatproofWorkbench>());
		base.SetDefaults();
	}
}
