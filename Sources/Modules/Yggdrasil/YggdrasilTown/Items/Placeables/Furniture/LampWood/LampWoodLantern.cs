namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.LampWood;

public class LampWoodLantern : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.LampWood.Furniture.LampWoodLantern>());
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<LampWood_Wood>(6);
		recipe.AddIngredient(ItemID.Torch, 1);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
