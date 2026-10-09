namespace Everglow.PlantAndFarm.Items.Materials;

public class GoldCup : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Golden Bell");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "风摆铃");
	}
	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 38;
		Item.maxStack = 999;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.material = true;
	}
}
