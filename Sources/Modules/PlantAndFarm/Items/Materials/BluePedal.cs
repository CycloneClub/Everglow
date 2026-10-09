namespace Everglow.PlantAndFarm.Items.Materials;

public class BluePedal : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Blue Borage");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "琉天苣");
	}
	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 32;
		Item.maxStack = 999;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.material = true;
	}
}
