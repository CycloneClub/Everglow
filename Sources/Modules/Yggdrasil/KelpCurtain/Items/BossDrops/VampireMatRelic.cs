namespace Everglow.Yggdrasil.KelpCurtain.Items.BossDrops;

public class VampireMatRelic : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.BossDrops.VampireMatRelic>(), 0);

		Item.width = 30;
		Item.height = 40;
		Item.rare = ItemRarityID.Master;
		Item.master = true;
		Item.value = Item.buyPrice(0, 5);
	}
}
