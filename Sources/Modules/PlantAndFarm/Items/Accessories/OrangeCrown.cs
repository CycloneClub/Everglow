namespace Everglow.PlantAndFarm.Items.Accessories;

public class OrangeCrown : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Tenacious Tulip");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "坚韧郁金香");
		// Tooltip.SetDefault("Increases defense by (max Hp * 3%)%\n'A tulip blessed by Aeolus, its structural strength is suprisingly high'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "防御力+(最大生命值*3%)%\n'埃俄罗斯祝福过的郁金香,结构强度出奇的高'");
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
		player.statDefense += (int)(player.statLifeMax2 * 0.03f);
	}
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<Materials.WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<Materials.OrangeTulip>(), 24)
			.AddTile(304)
			.Register();
	}
}
