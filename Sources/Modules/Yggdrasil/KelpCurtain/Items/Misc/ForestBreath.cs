namespace Everglow.Yggdrasil.KelpCurtain.Items.Misc;

public class ForestBreath : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Miscs;

	public override string Texture => $"Terraria/Images/Item_{ItemID.NaturesGift}";

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.value = Item.buyPrice(silver: 50);
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;
	}
}
