using Everglow.CagedDomain.Tiles;

namespace Everglow.CagedDomain.Items;

public class PierWithSlabsTop_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<PierWithSlabsTop>());
		Item.width = 50;
		Item.height = 34;
		Item.value = 20000;
	}

	public override void HoldItem(Player player)
	{
		Main.placementPreview = true;
	}

	public override bool CanUseItem(Player player)
	{
		var pWST = TileLoader.GetTile(ModContent.TileType<PierWithSlabsTop>()) as PierWithSlabsTop;
		if (pWST != null)
		{
			int x = (int)(Main.MouseWorld.X / 16);
			int y = (int)(Main.MouseWorld.Y / 16);
			if (pWST.NoCollideInSolidGrid(x, y, false))
			{
				pWST.PlaceAtTileObjectDataOrigin(x, y);
				Item.stack--;
				return false;
			}
		}
		return false;
	}
}
