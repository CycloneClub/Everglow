using Everglow.CagedDomain.Tiles;
using Terraria.GameContent.Creative;

namespace Everglow.CagedDomain.Items;

public class BarrierPlatform_Item : ModItem
{
	public override string LocalizationCategory => Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 200;
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<BarrierPlatform>());
		Item.width = 24;
		Item.height = 18;
		Item.useTime = 5;
		Item.useAnimation = 5;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(2);
		recipe.AddIngredient(ModContent.ItemType<BarrierBlock_Item>(), 1);
		recipe.Register();
	}
}
