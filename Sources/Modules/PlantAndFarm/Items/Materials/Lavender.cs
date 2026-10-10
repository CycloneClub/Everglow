namespace Everglow.PlantAndFarm.Items.Materials;

public class Lavender : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Lavender");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "紫笔头");
	}

	public override void SetDefaults()
	{
		Item.width = 12;
		Item.height = 34;
		Item.maxStack = 999;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.material = true;
	}
}
