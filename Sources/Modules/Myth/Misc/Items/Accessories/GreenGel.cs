namespace Everglow.Myth.Misc.Items.Accessories;

public class GreenGel : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 34;
		Item.height = 26;
		Item.value = 1563;
		Item.accessory = true;
		Item.rare = ItemRarityID.Orange;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.statLifeMax2 += 30;
		player.lifeRegen += 4;
	}
}
