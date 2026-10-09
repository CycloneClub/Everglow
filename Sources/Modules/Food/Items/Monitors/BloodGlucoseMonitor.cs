using Everglow.Food.InfoDisplays;

namespace Everglow.Food.Items.Monitors;

public class BloodGlucoseMonitor : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("血糖检测仪");
		// Tooltip.SetDefault("显示当前饱食度");
	}

	public override void SetDefaults()
	{

		Item.value = Item.buyPrice(50000);
		Item.rare = ItemRarityID.Green;
		Item.accessory = true;

	}

	public override void UpdateInventory(Player player)
	{
		FoodSatietyInfoDisplayplayer SatietyInfo = player.GetModPlayer<FoodSatietyInfoDisplayplayer>();
		SatietyInfo.AccBloodGlucoseMonitor = true;
	}
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		FoodSatietyInfoDisplayplayer SatietyInfo = player.GetModPlayer<FoodSatietyInfoDisplayplayer>();
		SatietyInfo.AccBloodGlucoseMonitor = true;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Wire, 10)
			.AddIngredient(ItemID.Lens, 5)
			.AddRecipeGroup(RecipeGroupID.IronBar, 10)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
