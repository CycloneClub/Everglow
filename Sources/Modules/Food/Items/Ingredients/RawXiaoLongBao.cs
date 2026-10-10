using Everglow.Food.Dusts;
using Everglow.Food.Tiles;

namespace Everglow.Food.Items.Ingredients;

public class RawXiaoLongBao : FoodIngredientItem
{
	public override void SetDefaults()
	{
		DefaultAsIngredient(50);
		SlicedItemType = ModContent.ItemType<GroundMeat>();
		SliceDustType = ModContent.DustType<SteakDust>();
	}

	public override void AddRecipes()
	{
		CreateRecipe(6)
			.AddIngredient(ModContent.ItemType<Flour>())
			.AddIngredient(ModContent.ItemType<GroundMeat>())
			.AddCondition(Condition.NearWater)
			.AddTile(ModContent.TileType<ChoppingBlock>())
			.Register();
	}
}
