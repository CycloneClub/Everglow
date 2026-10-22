namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.LampWood;

public class LampWoodSofa : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.LampWood.Furniture.LampWoodSofa>());
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<LampWood_Wood>(5);
		recipe.AddIngredient(ItemID.Silk, 2);
		recipe.AddTile(TileID.Sawmill);
		recipe.Register();
	}
}
