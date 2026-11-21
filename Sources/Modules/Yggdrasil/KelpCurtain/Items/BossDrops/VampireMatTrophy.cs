using BossDropTile = Everglow.Yggdrasil.KelpCurtain.Tiles.BossDrops.VampireMatTrophy;

namespace Everglow.Yggdrasil.KelpCurtain.Items.BossDrops;

public class VampireMatTrophy : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<BossDropTile>());

		Item.width = 32;
		Item.height = 32;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(0, 1);
	}
}
