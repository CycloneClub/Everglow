using Everglow.Food.InfoDisplays;

namespace Everglow.Food.Items.Monitors;

public class OsmoticPressureMonitor : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("渗透压检测仪");
		// Tooltip.SetDefault("显示渴觉状态");
	}

	public override void SetDefaults()
	{

		Item.value = Item.buyPrice(50000);
		Item.rare = ItemRarityID.Green;
		Item.accessory = true;
	}

	public override void UpdateInventory(Player player)
	{
		ThirstystateInfoDisplayplayer ThirstystateInfo = player.GetModPlayer<ThirstystateInfoDisplayplayer>();
		ThirstystateInfo.AccOsmoticPressureMonitor = true;
	}
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		ThirstystateInfoDisplayplayer ThirstystateInfo = player.GetModPlayer<ThirstystateInfoDisplayplayer>();
		ThirstystateInfo.AccOsmoticPressureMonitor = true;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Wire, 10)
			.AddIngredient(ItemID.Glass, 20)
			.AddRecipeGroup(RecipeGroupID.IronBar, 10)
			.AddTile(TileID.HeavyWorkBench)
			.Register();
	}
}
