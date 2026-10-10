using Everglow.Myth.Common;

namespace Everglow.Myth.Misc.Items.Accessories;

[AutoloadEquip(EquipType.Neck)]
public class RedGel : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 34;
		Item.height = 34;
		Item.value = 1824;
		Item.accessory = true;
		Item.rare = ItemRarityID.Orange;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetCritChance(DamageClass.Generic) += 6;
		MythContentPlayer mplayer = player.GetModPlayer<MythContentPlayer>();
		mplayer.CriticalDamage += 0.09f;
	}
}
