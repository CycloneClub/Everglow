using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood.Furniture;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.LampWood;

public class LampWoodCandelabra_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<LampWoodCandelabra>());
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<LampWood_Wood>(5);
		recipe.AddIngredient(ItemID.Torch, 3);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
