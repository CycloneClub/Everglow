namespace Everglow.PlantAndFarm.Items.Materials;

public class WindMoveSeed : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Aeolus' Seed");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "风动之种");
	}

	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 32;
		Item.value = 150;
		Item.rare = ItemRarityID.White;
		Item.material = true;
		Item.maxStack = 999;
	}
}
