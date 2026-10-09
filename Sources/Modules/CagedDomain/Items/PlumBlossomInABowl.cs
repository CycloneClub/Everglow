namespace Everglow.CagedDomain.Items;

public class PlumBlossomInABowl : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 34;
		Item.maxStack = 999;
		Item.value = 10000;
		Item.rare = ItemRarityID.Blue;
		Item.autoReuse = true;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.consumable = true;
		Item.createTile = ModContent.TileType<Tiles.PlumBlossomInABowl>();
		Item.placeStyle = 0;
		Item.useTime = 10;
		Item.useAnimation = 10;
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
