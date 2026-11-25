using BossDropTile = Everglow.Myth.TheTusk.Tiles.BossDrops.BloodyTuskTrophy;

namespace Everglow.Myth.TheTusk.Items.BossDrops;

public class BloodyTuskTrophy : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<BossDropTile>());

		Item.width = 32;
		Item.height = 32;
		Item.maxStack = 99;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.sellPrice(0, 1, 0, 0);
	}
}
