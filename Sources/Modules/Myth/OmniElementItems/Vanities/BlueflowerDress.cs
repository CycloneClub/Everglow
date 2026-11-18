namespace Everglow.Myth.OmniElementItems.Vanities;

[AutoloadEquip(EquipType.Legs)]
public class BlueflowerDress : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Vanity;

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 18;
		Item.value = Item.buyPrice(0, 1, 0, 0);
		Item.rare = ItemRarityID.Green;
	}
}
