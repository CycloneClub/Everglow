using Terraria.GameContent.Creative;

namespace Everglow.Myth.TheFirefly.Items.Furnitures;

public class GlowWoodClock : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furnitures.GlowWoodClock>());
		Item.width = 20;
		Item.height = 32;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ModContent.ItemType<GlowWood>(), 10);
		recipe.AddIngredient(ItemID.Glass, 6);
		recipe.AddIngredient(ItemID.IronBar, 3);
		recipe.AddRecipeGroup(RecipeGroups.IronBar, 2);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
