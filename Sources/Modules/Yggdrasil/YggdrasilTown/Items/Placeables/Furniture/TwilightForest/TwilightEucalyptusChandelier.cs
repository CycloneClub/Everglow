using Everglow.Commons.Templates.Furniture;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.TwilightForest;

public class TwilightEucalyptusChandelier : ChandelierItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.TwilightForest.Furnitures.TwilightEucalyptusChandelier>());
		base.SetDefaults();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<TwilightEucalyptusWood_Item>(4);
		recipe.AddIngredient(ItemID.Torch, 4);
		recipe.AddIngredient(ItemID.Chain, 1);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
