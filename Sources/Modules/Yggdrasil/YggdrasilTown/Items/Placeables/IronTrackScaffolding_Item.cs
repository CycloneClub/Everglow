namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

public class IronTrackScaffolding_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<YggdrasilTown.Walls.IronTrackScaffolding>());
		Item.width = 24;
		Item.height = 24;
	}
}
