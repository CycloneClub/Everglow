namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class ReekingBait : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.SummonItems;

	public override string Texture => $"Terraria/Images/Item_{ItemID.WormFood}";

	public override void SetDefaults()
	{
		Item.consumable = true;
		Item.width = 20;
		Item.height = 20;

		Item.value = Item.buyPrice(silver: 20);
		Item.rare = ItemRarityID.Blue;
		Item.maxStack = Item.CommonMaxStack;

		Item.useTime = Item.useAnimation = 12;
		Item.noMelee = true;
		Item.useTurn = true;
		Item.UseSound = SoundID.Roar;
		Item.useStyle = ItemUseStyleID.Swing;
	}

	public override bool CanUseItem(Player player)
	{
		return false;
	}
}
