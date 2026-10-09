namespace Everglow.PlantAndFarm.Items.Accessories;

public class CyanBranch : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Porcelianized Flower");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "一束天青");
		// Tooltip.SetDefault("Increases evade by 8 for 10s after struck\n'It's pretty hard, but also brittle'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "受击后10秒增加8闪避能力\n'它十分坚硬，但也很脆弱'");
	}
	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 42;
		Item.value = 3142;
		Item.accessory = true;
		Item.rare = 3;
	}
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// MythPlayer.CyanBranch = 2;
	}
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<Materials.WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<Materials.CyanHyacinth>(), 24)
			.AddTile(304)
			.Register();
	}
}
