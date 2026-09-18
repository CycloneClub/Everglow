namespace Everglow.Yggdrasil.KelpCurtain.Items.Placeables;

public class OldMoss_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.OldMoss>());
		Item.width = 16;
		Item.height = 16;
	}
}
