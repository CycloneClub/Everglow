using Everglow.Yggdrasil.YggdrasilTown.Walls.TwilightForest;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Walls;

public class GreenRelicWall_Style2_Item : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<GreenRelicWall_Style2>());
		Item.width = 24;
		Item.height = 24;
	}

	public override void AddRecipes()
	{
		CreateRecipe(4)
			.AddIngredient(ModContent.ItemType<GreenRelicBrick_Item>(), 1)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
