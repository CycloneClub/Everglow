namespace Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;

public class YggdrasilTownAccessCard : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Miscs;

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 28;
		Item.maxStack = 1;
		Item.rare = ItemRarityID.White;
		Item.value = 10000;
	}
}
