using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class GoldRound : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Marigoldisc");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "金盏轮盘");
		// Tooltip.SetDefault("7 defence\nIncreases damage by 7%\n'Traditional Terrarians usually wear this to keep them healthy'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "7防御\n伤害增加7%\n'传统的泰拉人通常配戴它来保持健康'");
	}

	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 32;
		Item.value = 3347;
		Item.accessory = true;
		Item.rare = 3;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetDamage(DamageClass.Generic) *= 1.07f;
		player.statDefense += 7;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<LightChrysanthemum>(), 24)
			.AddTile(304)
			.Register();
	}
}
