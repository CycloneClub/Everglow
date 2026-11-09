namespace Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;

public class StrongCorticalMembrane : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Miscs;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 24;

		Item.stack = Item.CommonMaxStack;

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(silver: 10);

		Item.questItem = true;
	}
}
