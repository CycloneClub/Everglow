namespace Everglow.PlantAndFarm.Items.Materials;

public class BlueFreeze : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Icy Poker");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "翠蓝朵");
	}
	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 30;
		Item.maxStack = 999;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.material = true;
	}
}
