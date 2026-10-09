namespace Everglow.Yggdrasil.YggdrasilTown.Items.BossDrops;

public class SquamousShellRelic : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		// Vanilla has many useful methods like these, use them! This substitutes setting Item.createTile and Item.placeStyle aswell as setting a few values that are common across all placeable items
		// The place style (here by default 0) is important if you decide to have more than one relic share the same tile type (more on that in the tiles' code)
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.BossDrops.SquamousShellRelic>(), 0);

		Item.width = 30;
		Item.height = 40;
		Item.rare = ItemRarityID.Master;
		Item.master = true; // This makes sure that "Master" displays in the tooltip, as the rarity only changes the item name color
		Item.value = Item.buyPrice(0, 5);

		// Item.width = 38;
		// Item.height = 50;
		// Item.useAnimation = 20;
		// Item.master = true;
		// Item.useTime = 20;
		// Item.maxStack = 99;
		// Item.rare = ItemRarityID.White;
		// Item.value = Item.buyPrice(0, 1, 0, 0);
		// Item.useAnimation = 15;
		// Item.useTime = 10;
		// Item.useStyle = ItemUseStyleID.Swing;
		// Item.consumable = true;
		// Item.useTurn = true;
		// Item.autoReuse = true;
		// Item.createTile = ModContent.TileType<Tiles.BossDrops.SquamousShellRelic>();
	}
}
