using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class PurpleReedHalo : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Purple Reed Headband");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "紫苇花冠");
		// Tooltip.SetDefault("Increases max Hp by 30\nIncreases max mana by 40\n'smells good and looks good'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "生命上限增加30\n魔力上限增加40\n'好闻又好看'");
	}

	public override void SetDefaults()
	{
		Item.width = 46;
		Item.height = 36;
		Item.value = 2877;
		Item.accessory = true;
		Item.rare = ItemRarityID.Orange;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.statLifeMax2 += 30;
		player.statManaMax2 += 40;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<PurpleTail>(), 24)
			.AddTile(TileID.LivingLoom)
			.Register();
	}
}
