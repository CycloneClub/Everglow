namespace Everglow.CagedDomain.Items;

public class PlumBlossomInABowl : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.PlumBlossomInABowl>());
		Item.width = 26;
		Item.height = 34;
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
