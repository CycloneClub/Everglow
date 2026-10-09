using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class WhiteWoodFlower : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Snow Ear Flower");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "银耳花");
		// Tooltip.SetDefault("Increases minion slots by 2\n'Fantastic symbiosis'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "召唤栏位增加2\n'奇妙的共生关系'");
	}
	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 16;
		Item.value = 3223;
		Item.accessory = true;
		Item.rare = 3;
	}
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.maxMinions += 2;
	}
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<SilverClock>(), 24)
			.AddTile(304)
			.Register();
	}
}
