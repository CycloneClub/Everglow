using Terraria.Localization;

namespace Everglow.Myth.TheTusk.Items.Accessories;

[AutoloadEquip(EquipType.Neck)]
public class TuskLace : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(5, 3, 3);

	public override void SetDefaults()
	{
		Item.width = 44;
		Item.height = 46;
		Item.value = 2000;
		Item.accessory = true;
		Item.rare = ItemRarityID.Green;
		// Item.vanity = true;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetArmorPenetration(DamageClass.Generic) += 5;
		player.GetCritChance(DamageClass.Generic) += 3;
		player.GetDamage(DamageClass.Generic) *= 1.03f;
	}
}
