namespace Everglow.Myth.TheTusk.Items.BossDrops;

public class TuskRelic : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.BossDrops.TuskRelic>());
		Item.width = 38;
		Item.height = 50;
		Item.master = true;
		Item.maxStack = 99;
		Item.rare = ItemRarityID.White;
		Item.value = Item.sellPrice(0, 1, 0, 0);
	}
}
