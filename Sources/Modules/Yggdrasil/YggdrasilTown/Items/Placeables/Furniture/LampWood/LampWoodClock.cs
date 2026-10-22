namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.LampWood;

public class LampWoodClock : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.LampWood.Furniture.LampWoodClock>());
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<LampWood_Wood>(10);
		recipe.AddRecipeGroup(RecipeGroupID.IronBar, 3);
		recipe.AddIngredient(ItemID.Glass, 6);
		recipe.AddTile(TileID.Sawmill);
		recipe.Register();
	}
}
