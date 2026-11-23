using Everglow.Yggdrasil.YggdrasilTown.Walls;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Walls;

public class StoneDragonScaleWoodWall_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<StoneDragonScaleWoodWall_Unsafe_Item>();
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableWall(ModContent.WallType<StoneDragonScaleWoodWall>());
		Item.width = 24;
		Item.height = 24;
	}

	public override void AddRecipes()
	{
		CreateRecipe(4)
			.AddIngredient(ModContent.ItemType<StoneDragonScaleWood>(), 1)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
