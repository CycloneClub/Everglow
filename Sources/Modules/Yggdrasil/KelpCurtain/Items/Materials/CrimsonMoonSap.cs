namespace Everglow.Yggdrasil.KelpCurtain.Items.Materials;

public class CrimsonMoonSap : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 28;
		Item.value = 5000;
		Item.maxStack = Item.CommonMaxStack;
		Item.rare = ItemRarityID.Orange;
	}
}
