using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class WhiteCrown : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Flower of Hope");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "希望之花");
		// Tooltip.SetDefault("Increases running speed by (max Hp * 0.2%)\n'A plain white flower, seems to mourn for someone'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "奔跑速度增加(生命上限*0.2%)\n'一枝素白的郁金香，像是在悼念某人'");

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
		player.maxRunSpeed += (int)(player.statLifeMax2 * 0.02f) / 10f;
	}
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<WhiteTulip>(), 24)
			.AddTile(304)
			.Register();
	}
}
