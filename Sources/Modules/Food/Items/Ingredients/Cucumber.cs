using Everglow.Food.Dusts;

namespace Everglow.Food.Items.Ingredients;

public class Cucumber : FoodIngredientItem
{
	public override void SetDefaults()
	{
		DefaultAsIngredient(100);
		SlicedItemType = ModContent.ItemType<CucumberPieces>();
		SliceDustType = ModContent.DustType<CucumberDust>();
	}
}
