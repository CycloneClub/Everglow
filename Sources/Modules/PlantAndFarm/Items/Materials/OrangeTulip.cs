namespace Everglow.PlantAndFarm.Items.Materials;

public class OrangeTulip : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 32;
		Item.maxStack = Item.CommonMaxStack;
		Item.value = 0;
		Item.rare = ItemRarityID.White;
		Item.material = true;
	}
}
