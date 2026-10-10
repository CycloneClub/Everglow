using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

public class FurnaceCopperPipe_Large_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public int State = 0;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<FurnaceCopperPipe_Large_V>());
		Item.width = 16;
		Item.height = 16;
	}

	public override void HoldItem(Player player)
	{
		if (Main.mouseRight && Main.mouseRightRelease)
		{
			State += 1;
			State %= 2;
		}
		if (State == 0)
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<FurnaceCopperPipe_Large_H>());
			Item.placeStyle = 3;
		}
		else if (State == 1)
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<FurnaceCopperPipe_Large_V>());
			Item.placeStyle = 3;
		}
	}
}
