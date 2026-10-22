namespace Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

public class YggdrasilAmber : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.maxStack = Item.CommonMaxStack;
		Item.width = 24;
		Item.height = 22;
		Item.rare = ItemRarityID.White;
	}
}
