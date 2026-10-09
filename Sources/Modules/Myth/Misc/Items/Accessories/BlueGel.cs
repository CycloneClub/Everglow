namespace Everglow.Myth.Misc.Items.Accessories;

[AutoloadEquip(EquipType.Neck)]
public class BlueGel : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 34;
		Item.height = 26;
		Item.value = 1342;
		Item.accessory = true;
		Item.rare = ItemRarityID.Orange;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.statManaMax2 += 30;
		player.manaRegen += 4;
	}
}
