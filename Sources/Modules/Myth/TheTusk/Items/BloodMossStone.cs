using Terraria.Localization;

namespace Everglow.Myth.TheTusk.Items;

public class BloodMossStone : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.BloodMossStone>());
		Item.width = 16;
		Item.height = 16;
		Item.maxStack = 999;
	}
}
