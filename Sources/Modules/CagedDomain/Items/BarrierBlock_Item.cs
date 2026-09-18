using Everglow.CagedDomain.Tiles;

namespace Everglow.CagedDomain.Items;

public class BarrierBlock_Item : ModItem
{
	public override string LocalizationCategory => Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<BarrierBlock>());
		Item.useTime = 5;
		Item.useAnimation = 5;
	}
}
