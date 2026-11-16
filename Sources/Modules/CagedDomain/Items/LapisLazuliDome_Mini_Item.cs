using Everglow.CagedDomain.Tiles;

namespace Everglow.CagedDomain.Items;

public class LapisLazuliDome_Mini_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<LapisLazuliDome_Mini>());
		Item.width = 36;
		Item.height = 14;
	}
}
