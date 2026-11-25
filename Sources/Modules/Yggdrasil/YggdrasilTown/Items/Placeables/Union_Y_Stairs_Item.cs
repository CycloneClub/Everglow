using Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

public class Union_Y_Stairs_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Union_Y_Stairs>());
		Item.width = 30;
		Item.height = 30;
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
	}

	public override void HoldItem(Player player)
	{
		Main.placementPreview = true;
	}

	public override bool CanUseItem(Player player)
	{
		var uStair = TileLoader.GetTile(ModContent.TileType<Union_Y_Stairs>()) as Union_Y_Stairs;
		if (uStair != null)
		{
			int x = (int)(Main.MouseWorld.X / 16 - 19);
			int y = (int)(Main.MouseWorld.Y / 16 - 19);
			uStair.PlaceOriginAtTopLeft(x, y);

			Item.stack--;
			return false;
		}
		return false;
	}

	public override bool? UseItem(Player player)
	{
		return false;
	}
}
