namespace Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

public class NylonRope : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 38;
		Item.value = 3;
		Item.maxStack = Item.CommonMaxStack;
	}
}
