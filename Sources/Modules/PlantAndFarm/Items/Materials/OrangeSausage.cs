namespace Everglow.PlantAndFarm.Items.Materials;

public class OrangeSausage : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Orange Pennisetum");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "狐绒草");
	}

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 34;
		Item.maxStack = 999;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.material = true;
	}
}
