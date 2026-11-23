using Terraria.Localization;

namespace Everglow.Myth.TheTusk.Items;

public class BloodyStoneWall : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<Walls.BloodyStoneWall>());
		Item.width = 20;
		Item.height = 20;
		Item.maxStack = 999;
	}
}
