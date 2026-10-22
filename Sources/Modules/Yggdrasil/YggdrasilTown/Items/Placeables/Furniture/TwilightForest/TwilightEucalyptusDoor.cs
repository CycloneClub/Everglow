using Everglow.Commons.Templates.Furniture;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.Furnitures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.TwilightForest;

public class TwilightEucalyptusDoor : DoorItem
{
	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<TwilightEucalyptusDoorClosed>());
		base.SetDefaults();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<TwilightEucalyptusWood_Item>(6);
		recipe.AddTile(TileID.WorkBenches);
		recipe.Register();
	}
}
