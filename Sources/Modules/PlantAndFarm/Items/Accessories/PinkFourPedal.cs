using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class PinkFourPedal : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Pink Samsara");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "四命轮回");
		// Tooltip.SetDefault("Increases max Hp by 40\n'It's said that injured animals will eat it to recover...it's true, but it seems not to be effective to human'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "最大生命值增加40\n'据说受伤的动物会去吃它来疗伤...事实确实如此,但好像对人效果不大'");
	}
	public override void SetDefaults()
	{
		Item.width = 10;
		Item.height = 26;
		Item.value = 2985;
		Item.accessory = true;
		Item.rare = 3;
	}
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.statLifeMax2 += 40;
	}
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<PinkSun>(), 24)
			.AddTile(304)
			.Register();
	}
}
