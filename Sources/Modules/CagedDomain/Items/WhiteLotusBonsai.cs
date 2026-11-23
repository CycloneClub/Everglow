namespace Everglow.CagedDomain.Items;

public class WhiteLotusBonsai : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.WhiteLotusBonsai>());
		Item.width = 32;
		Item.height = 64;
		Item.maxStack = 999;
		Item.value = 10000;
		Item.rare = ItemRarityID.Blue;
	}

	public override bool? UseItem(Player player)
	{
		return base.UseItem(player);
	}

	public override void HoldItem(Player player)
	{
		Item.placeStyle = Math.Max(player.direction, 0);
		base.HoldItem(player);
	}
}
