using Terraria.GameContent.Creative;
using Everglow.Commons.Utilities;
using Everglow.Commons.Templates.Furniture;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.Furnace;

public class HeatproofCandelabra_Item : CandelabraItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.FurnaceTiles.HeatproofCandelabra>());
		base.SetDefaults();
	}
}
