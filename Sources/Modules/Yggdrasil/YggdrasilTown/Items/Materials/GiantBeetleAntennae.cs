namespace Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

public class GiantBeetleAntennae : ModItem
{
	public override void SetDefaults()
	{
		Item.height = 22;
		Item.width = 32;
		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(0, 0, 0, 0);
		Item.maxStack = Item.CommonMaxStack;
	}
}
