namespace Everglow.Myth.Misc.Items.Accessories;

[AutoloadEquip(EquipType.Neck)]
public class DukeTooth : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 38;
		Item.height = 48;
		Item.value = 26090;
		Item.accessory = true;
		Item.rare = ItemRarityID.Lime;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetArmorPenetration(DamageClass.Generic) += 12;
	}
}
