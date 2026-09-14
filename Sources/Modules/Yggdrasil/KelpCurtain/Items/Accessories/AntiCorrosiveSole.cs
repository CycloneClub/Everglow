using Everglow.Yggdrasil.KelpCurtain.Buffs;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Accessories;

public class AntiCorrosiveSole : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.SetNameOverride("Anti-corrosive Sole");

		Item.width = 28;
		Item.height = 38;

		Item.accessory = true;

		Item.value = Item.sellPrice(gold: 1);
		Item.rare = ItemRarityID.Green;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.moveSpeed += 0.05f;
		player.buffImmune[ModContent.BuffType<CorrosiveDebuff>()] = true;
	}
}
