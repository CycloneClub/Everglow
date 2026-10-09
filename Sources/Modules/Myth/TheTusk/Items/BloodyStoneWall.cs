using Terraria.Localization;

namespace Everglow.Myth.TheTusk.Items;

public class BloodyStoneWall : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Bloody Stone Wall");
	}

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.maxStack = 999;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.useAnimation = 15;
		Item.useTime = 7;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.consumable = true;
		Item.createWall = ModContent.WallType<Walls.BloodyStoneWall>();
	}
}
