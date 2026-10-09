using Everglow.Food.Dusts;

namespace Everglow.Food.Items.Ingredients;

public class Potato : FoodIngredientItem
{
	public override void SetDefaults()
	{
		DefaultAsIngredient(100);
		SlicedItemType = ModContent.ItemType<GroundPotato>();
		SliceDustType = ModContent.DustType<PotatoDust>();
	}
}
