using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class CyanPedal : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Four-pointed Borage Flower");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "四瓣琉璃");
		// Tooltip.SetDefault("Increases evade by 4\n'The missing petal was converted to your braveness'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "闪避能力增加4\n'少的那一瓣化作你的勇气'");
	}
	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 22;
		Item.value = 3068;
		Item.accessory = true;
		Item.rare = 3;
	}
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// MythPlayer.CyanPedal = 2;
	}
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<BluePedal>(), 24)
			.AddTile(304)
			.Register();
	}
}
