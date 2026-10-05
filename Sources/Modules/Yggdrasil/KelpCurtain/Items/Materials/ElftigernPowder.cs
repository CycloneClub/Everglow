namespace Everglow.Yggdrasil.KelpCurtain.Items.Materials;

public class ElftigernPowder : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Materials;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 24;
		Item.value = Item.buyPrice(silver: 25);
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 15;
		Item.useAnimation = 15;
	}

	public override bool CanUseItem(Player player) => false;
}
