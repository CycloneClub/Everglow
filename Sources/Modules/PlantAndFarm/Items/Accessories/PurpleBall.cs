namespace Everglow.PlantAndFarm.Items.Accessories;

public class PurpleBall : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Purple Hydrangea Pandents");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "紫绣球吊坠");
		// Tooltip.SetDefault("Hitting enemies around you launches an explosion of petals\nDamage of the petals increases with damage of the attack\nHas a 0.5s CD\n'Angry Hydrangea Purpura'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "命中身周的敌人会释放紫色花瓣爆炸\n花瓣伤害随攻击的伤害增加\n有0.5秒冷却\n'愤怒紫绣球'");
	}

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 24;
		Item.value = 3226;
		Item.accessory = true;
		Item.rare = 3;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// MythPlayer.PurpleBallFlower = 2;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<Materials.WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<Materials.LightPurpleBalls>(), 24)
			.AddTile(304)
			.Register();
	}
}
