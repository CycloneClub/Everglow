using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class SilverCupFlower : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Royal Goblet");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "皇家酒杯");
		// Tooltip.SetDefault("8 defense\nIncreases max Hp by 20\n'There was a vicious king who has really tried to use it as a goblet, he failed to drink anything'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "8防御\n生命上限增加20\n'曾有一位残暴的国王真的尝试拿它当酒杯,他一滴也没喝上'");
	}
	public override void SetDefaults()
	{
		Item.width = 14;
		Item.height = 24;
		Item.value = 2907;
		Item.accessory = true;
		Item.rare = 3;
	}
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.statDefense += 8;
		player.statLifeMax2 += 20;
	}
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<GoldCup>(), 24)
			.AddTile(304)
			.Register();
	}
}
