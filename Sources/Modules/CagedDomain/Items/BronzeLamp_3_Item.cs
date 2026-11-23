using Everglow.CagedDomain.Tiles;

namespace Everglow.CagedDomain.Items;

public class BronzeLamp_3_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<BronzeLamp_3>());
		Item.width = 10;
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
		BronzeLamp_3 bronzeLamp_3 = TileLoader.GetTile(ModContent.TileType<BronzeLamp_3>()) as BronzeLamp_3;
		if (bronzeLamp_3 != null)
		{
			int x = (int)(Main.MouseWorld.X / 16 - 3);
			int y = (int)(Main.MouseWorld.Y / 16);
			if (bronzeLamp_3.CanPlaceAtBottomLeft(x, y))
			{
				bronzeLamp_3.PlaceOriginAtBottomLeft(x, y);
				Item.stack--;
				return false;
			}
		}
		return false;
	}

	public override bool? UseItem(Player player)
	{
		return false;
	}
}
