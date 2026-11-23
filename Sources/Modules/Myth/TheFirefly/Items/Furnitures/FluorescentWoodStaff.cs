using Terraria.GameContent.Creative;

namespace Everglow.Myth.TheFirefly.Items.Furnitures;

public class FluorescentWoodStaff : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.LifeFluorescentTreeWood>());
		Item.width = 48;
		Item.height = 48;
		Item.tileWand = ModContent.ItemType<GlowWood>();
		Item.value = 2000;
		Item.consumable = false;
		Item.maxStack = 1;
	}
}
