using Everglow.CagedDomain.Tiles;

namespace Everglow.CagedDomain.Items;

public class FluoriteLampPost_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<FluoriteLampPost>());
		Item.height = 48;
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
	}

	public override void HoldItem(Player player)
	{
		Main.placementPreview = true;
	}

	public override bool CanUseItem(Player player)
	{
		FluoriteLampPost fluoriteLampPost = TileLoader.GetTile(ModContent.TileType<FluoriteLampPost>()) as FluoriteLampPost;
		if (fluoriteLampPost != null)
		{
			int x = (int)(Main.MouseWorld.X / 16 - 1);
			int y = (int)(Main.MouseWorld.Y / 16);
			if (fluoriteLampPost.CanPlaceAtBottomLeft(x, y))
			{
				if (fluoriteLampPost.CanPlaceAtBottomLeft(x, y))
				{
					fluoriteLampPost.PlaceOriginAtBottomLeft(x, y);
					Item.stack--;
					return false;
				}
			}
		}
		return false;
	}

	public override bool? UseItem(Player player)
	{
		return false;
	}
}
