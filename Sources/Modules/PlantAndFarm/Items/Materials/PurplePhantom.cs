namespace Everglow.PlantAndFarm.Items.Materials;

public class PurplePhantom : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Phantom Orchid");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "幻蝶兰");
	}

	public override void SetDefaults()
	{
		Item.width = 34;
		Item.height = 30;
		Item.maxStack = 999;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.material = true;
	}
}
