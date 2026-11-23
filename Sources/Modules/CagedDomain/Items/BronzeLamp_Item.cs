using Everglow.CagedDomain.Tiles;

namespace Everglow.CagedDomain.Items;

public class BronzeLamp_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<BronzeLamp>());
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
		BronzeLamp bronzeLamp = TileLoader.GetTile(ModContent.TileType<BronzeLamp>()) as BronzeLamp;
		if (bronzeLamp != null)
		{
			int x = (int)(Main.MouseWorld.X / 16 - 1);
			int y = (int)(Main.MouseWorld.Y / 16);
			if (bronzeLamp.CanPlaceAtBottomLeft(x, y))
			{
				if (bronzeLamp.CanPlaceAtBottomLeft(x, y))
				{
					bronzeLamp.PlaceOriginAtBottomLeft(x, y);
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
