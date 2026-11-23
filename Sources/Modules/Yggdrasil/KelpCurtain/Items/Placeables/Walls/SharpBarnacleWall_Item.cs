using Everglow.Yggdrasil.KelpCurtain.Walls;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Placeables.Walls;

public class SharpBarnacleWall_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SharpBarnacleWall_Unsafe_Item>();
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<SharpBarnacleWall>());
		Item.width = 24;
		Item.height = 24;
	}

	public override void AddRecipes()
	{
		CreateRecipe(4)
			.AddIngredient(ModContent.ItemType<SharpBarnacleLayer_Item>(), 1)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
