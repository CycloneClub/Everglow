using Everglow.CagedDomain.Tiles.Elevators;

namespace Everglow.CagedDomain.Items.Elevators;

public class LivingWoodElevator_Item : ModItem
{
	public override string LocalizationCategory => Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<LivingWoodElevator_Winch>());
		Item.width = 42;
		Item.height = 64;
		Item.value = 20000;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Wood, 40)
			.AddIngredient(RecipeGroupID.IronBar, 10)
			.AddIngredient(ItemID.Chain, 20)
			.AddIngredient(ItemID.Wire, 5)
			.AddTile(TileID.HeavyWorkBench)
			.AddTile(TileID.LivingLoom)
			.Register();
	}
}
