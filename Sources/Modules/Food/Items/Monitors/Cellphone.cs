using Everglow.Food.InfoDisplays;

namespace Everglow.Food.Items.Monitors;

public class Cellphone : GlobalItem
{
	public override void UpdateInventory(Item item, Player player)
	{
		if (item.type == ItemID.CellPhone || item.type is 5358 or 5359 or 5360 or 5361) // CellPhone or any ShellPhone
		{
			FoodSatietyInfoDisplayplayer SatietyInfo = player.GetModPlayer<FoodSatietyInfoDisplayplayer>();
			SatietyInfo.AccBloodGlucoseMonitor = true; // 显示当前饱食度

			ThirstystateInfoDisplayplayer ThirstystateInfo = player.GetModPlayer<ThirstystateInfoDisplayplayer>();
			ThirstystateInfo.AccOsmoticPressureMonitor = true; // 显示渴觉状态
		}
	}
}
public class CellphoneRecipe : ModSystem
{
	public override void PostAddRecipes()
	{
		for (int i = 0; i < Recipe.numRecipes; i++)
		{
			Recipe recipe = Main.recipe[i];
			if (recipe.TryGetIngredient(ItemID.PDA, out _))
			{
				recipe.AddIngredient(ModContent.ItemType<OsmoticPressureMonitor>());
				recipe.AddIngredient(ModContent.ItemType<BloodGlucoseMonitor>());
			}
		}
	}
}
