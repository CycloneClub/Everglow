namespace Everglow.Myth.LanternMoon.Items;

public class FireCracker : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Miscs;

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 50;
		Item.value = 10000;
		Item.maxStack = Item.CommonMaxStack;
	}
}
