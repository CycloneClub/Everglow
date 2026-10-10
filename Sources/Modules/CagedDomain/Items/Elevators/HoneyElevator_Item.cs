using Everglow.CagedDomain.Tiles.Elevators;

namespace Everglow.CagedDomain.Items.Elevators;

public class HoneyElevator_Item : ModItem
{
	public override string LocalizationCategory => Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<HoneyElevator_Winch>());
		Item.width = 42;
		Item.height = 64;
		Item.value = 20000;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Wood, 40)
			.AddRecipeGroup(RecipeGroups.IronBar, 10)
			.AddIngredient(ItemID.Chain, 20)
			.AddIngredient(ItemID.Wire, 5)
			.AddTile(TileID.HeavyWorkBench)
			.Register();
	}
}
