namespace Everglow.PlantAndFarm.Items.Materials;

public class PinkSun : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Pink Thistle");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "酱粉蓟");
	}

	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 32;
		Item.maxStack = 999;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.material = true;
	}
}
