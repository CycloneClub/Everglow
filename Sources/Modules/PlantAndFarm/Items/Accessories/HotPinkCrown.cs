using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class HotPinkCrown : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Vitalized Tulip");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "活化郁金香");
		// Tooltip.SetDefault("Increases crit damage by (Max Hp * 5%)%\n'Catalyzed by Essence of Wind, now it will react to vitality'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "暴击伤害增加(最大生命值*5%)%\n'经过风之精华的催化,它将对生命力做出反应'");
	}
	public override void SetDefaults()
	{
		Item.width = 36;
		Item.height = 38;
		Item.value = 3920;
		Item.accessory = true;
		Item.rare = 3;
	}
	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// MythPlayer.AddCritDamage += player.statLifeMax2 * 0.05f / 100f;
	}
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<HotPinkTulip>(), 24)
			.AddTile(304)
			.Register();
	}
}
