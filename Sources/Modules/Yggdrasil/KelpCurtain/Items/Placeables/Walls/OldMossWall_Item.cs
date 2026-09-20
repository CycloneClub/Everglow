using Everglow.Yggdrasil.KelpCurtain.Walls;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Placeables.Walls;

public class OldMossWall_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<OldMossWall>());
		Item.width = 24;
		Item.height = 24;
	}

	public override void AddRecipes()
	{
		CreateRecipe(4)
			.AddIngredient(ModContent.ItemType<OldMoss_Item>(), 1)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
