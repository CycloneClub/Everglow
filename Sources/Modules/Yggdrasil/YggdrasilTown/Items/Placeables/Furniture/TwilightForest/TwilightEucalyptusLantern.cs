using Everglow.Commons.Templates.Furniture;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.TwilightForest;

public class TwilightEucalyptusLantern : LanternItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.TwilightForest.Furnitures.TwilightEucalyptusLantern>());
		base.SetDefaults();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<TwilightEucalyptusWood_Item>(6);
		recipe.AddIngredient(ItemID.Torch, 1);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
