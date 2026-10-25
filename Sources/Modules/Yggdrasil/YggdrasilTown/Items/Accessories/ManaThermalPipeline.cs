using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class ManaThermalPipeline : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MagicDamageBonus * 100f, ManaRegenIncrease);

	public const float MagicDamageBonus = 0.05f;
	public const int ManaRegenIncrease = 4;

	public override void SetDefaults()
	{
		Item.width = 34;
		Item.height = 34;

		Item.accessory = true;

		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(silver: 85);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetDamage<MagicDamageClass>() += MagicDamageBonus;
		player.manaRegen += ManaRegenIncrease;
	}
}
