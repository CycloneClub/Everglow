using Everglow.Yggdrasil.YggdrasilTown.Walls;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Walls;

public class GravelStoreWall_Thick_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<GravelStoreWall_Thick>());
		Item.width = 24;
		Item.height = 24;
	}
}
