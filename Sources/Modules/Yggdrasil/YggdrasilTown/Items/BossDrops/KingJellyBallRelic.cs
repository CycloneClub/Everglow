namespace Everglow.Yggdrasil.YggdrasilTown.Items.BossDrops;

public class KingJellyBallRelic : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.BossDrops.KingJellyBallRelic>(), 0);

		Item.width = 30;
		Item.height = 40;
		Item.rare = ItemRarityID.Master;
		Item.master = true;
		Item.value = Item.buyPrice(0, 5);
	}
}
