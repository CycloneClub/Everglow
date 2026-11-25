using Everglow.Myth.LanternMoon.Tiles.BossDrops;

namespace Everglow.Myth.LanternMoon.Items.BossDrops;

public class Relic_LanternGhostKing : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Relic_LanternGhostKing_Tile>());
		Item.width = 38;
		Item.height = 50;
		Item.master = true;
		Item.rare = ItemRarityID.White;
		Item.value = Item.sellPrice(0, 1, 0, 0);
	}
}
