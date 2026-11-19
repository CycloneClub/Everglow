namespace Everglow.PlantAndFarm.Items.Materials;

public class GoldCup : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 38;
		Item.maxStack = Item.CommonMaxStack;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.material = true;
	}
}
