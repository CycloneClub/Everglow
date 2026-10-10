namespace Everglow.PlantAndFarm.Items.Materials;

public class ShallowNight : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Heavenly Bloom");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "琼霄花");
	}

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 38;
		Item.maxStack = 999;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.material = true;
	}
}
