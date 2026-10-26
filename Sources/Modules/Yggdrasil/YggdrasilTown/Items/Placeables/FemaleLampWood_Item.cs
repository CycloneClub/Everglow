namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

public class FemaleLampWood_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.LampWood.FemaleLampWood>());
		Item.width = 16;
		Item.height = 16;
	}
}
