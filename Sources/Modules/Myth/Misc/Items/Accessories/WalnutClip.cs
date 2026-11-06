using Everglow.Myth.Common;
using Terraria.Localization;

namespace Everglow.Myth.Misc.Items.Accessories;

[AutoloadEquip(EquipType.Neck)]
public class WalnutClip : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(16, 50, 4);

	public override void SetDefaults()
	{
		Item.width = 32;
		Item.height = 32;
		Item.value = 21092;
		Item.accessory = true;
		Item.rare = ItemRarityID.Yellow;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		MythContentPlayer mplayer = player.GetModPlayer<MythContentPlayer>();
		mplayer.CriticalDamage += 0.16f;
		if (player.statLifeMax2 / 2f > player.statLife)
		{
			player.GetDamage(DamageClass.Generic) *= (player.statLifeMax2 / 2f - player.statLife) / 400f + 1;
		}
	}
}
