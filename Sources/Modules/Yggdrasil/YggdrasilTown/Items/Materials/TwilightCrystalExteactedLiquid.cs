namespace Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

public class TwilightCrystalExteactedLiquid : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 24;
		Item.maxStack = Item.CommonMaxStack;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient(ItemID.BottledWater, 1);
		recipe.AddIngredient<TwilightCrystal>(1);
		recipe.AddTile(TileID.Bottles);
		recipe.Register();
	}
}
