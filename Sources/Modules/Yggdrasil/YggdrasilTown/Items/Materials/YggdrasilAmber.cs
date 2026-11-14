namespace Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

public class YggdrasilAmber : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.YggdrasilAmber>());
		Item.width = 26;
		Item.height = 30;
	}
}
