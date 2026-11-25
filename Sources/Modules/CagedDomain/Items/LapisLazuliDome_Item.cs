using Everglow.CagedDomain.Tiles;

namespace Everglow.CagedDomain.Items;

public class LapisLazuliDome_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<LapisLazuliDome>());
		Item.width = 48;
		Item.height = 22;
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
	}

	public override void HoldItem(Player player)
	{
		Main.placementPreview = true;
	}

	public override bool CanUseItem(Player player)
	{
		LapisLazuliDome lapisLazuliDome = TileLoader.GetTile(ModContent.TileType<LapisLazuliDome>()) as LapisLazuliDome;
		if (lapisLazuliDome != null)
		{
			int x = (int)(Main.MouseWorld.X / 16 - 8);
			int y = (int)(Main.MouseWorld.Y / 16);
			if (lapisLazuliDome.CanPlaceAtBottomLeft(x, y))
			{
				if (lapisLazuliDome.CanPlaceAtBottomLeft(x, y))
				{
					lapisLazuliDome.PlaceOriginAtBottomLeft(x, y);
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
