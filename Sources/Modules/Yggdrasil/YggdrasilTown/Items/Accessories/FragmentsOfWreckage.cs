using Everglow.Commons.Mechanics.PlayerBonuses;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class FragmentsOfWreckage : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LifeLossForBonusMax, DamageBonus * 100f, MeleeSpeedBonus * 100f, DefenseBonus, PickSpeedBonus * 100f);

	public const float LifeLossForBonusMax = 120f;
	public const float DamageBonus = 0.06f;
	public const float MeleeSpeedBonus = 0.06f;
	public const float PickSpeedBonus = 0.1f;
	public const int DefenseBonus = 8;

	public override void SetDefaults()
	{
		Item.accessory = true;
		Item.width = 34;
		Item.height = 30;

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(platinum: 0, gold: 1);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// 1. Slightly increases stats based on life loss
		// ==============================================
		// A maximum of 120 life loss will be counted
		// +6% damage
		// +6% melee speed
		// +8 defense
		// +10% pick speed
		player.GetModPlayer<StatBonusPlayer>().FragmentsOfWreckage = true;
	}
}
