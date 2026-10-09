namespace Everglow.Myth.OmniElementItems.Vanities;

// TODO Need Rewrite
[AutoloadEquip(EquipType.Body)]
public class BlueflowerBra : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 12;
		Item.value = Item.buyPrice(0, 1, 0, 0);
		Item.rare = ItemRarityID.Green;
		Item.vanity = true;
		Item.accessory = true;
	}
}
