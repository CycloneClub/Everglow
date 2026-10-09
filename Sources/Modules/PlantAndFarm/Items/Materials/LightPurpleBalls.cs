namespace Everglow.PlantAndFarm.Items.Materials;

public class LightPurpleBalls : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("\"Purple Balls\"");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "暮色绒");
	}
	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 34;
		Item.maxStack = 999;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.material = true;
	}
}
