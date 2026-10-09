using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class DarkHat : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Ornamental Poppy");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "罂粟花坠饰");
		// Tooltip.SetDefault("Increases crit chance by 4% and crit damage by 12%\n'Not recommanded to use except for combat or decoration purpose'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "暴击率增加4%,暴击伤害增加12%\n'除用于战斗和装饰不建议使用'");
	}
	public override void SetDefaults()
	{
		Item.width = 24;
		Item.height = 24;
		Item.value = 3680;
		Item.accessory = true;
		Item.rare = 3;
	}
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetCritChance(DamageClass.Generic) += 4;
		// MythPlayer.AddCritDamage += 0.12f;
	}
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<DarkPoppy>(), 24)
			.AddTile(304)
			.Register();
	}
}
