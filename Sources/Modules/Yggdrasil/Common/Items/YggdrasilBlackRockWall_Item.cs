using Everglow.Yggdrasil.Common.Walls;

namespace Everglow.Yggdrasil.Common.Items;

public class YggdrasilBlackRockWall_Item : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<YggdrasilBlackRockWall_Unsafe_Item>();
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<YggdrasilBlackRockWall>());
		Item.width = 24;
		Item.height = 24;
	}

	public override void AddRecipes()
	{
		CreateRecipe(4)
			.AddIngredient(ModContent.ItemType<YggdrasilBlackRock_Item>(), 1)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
