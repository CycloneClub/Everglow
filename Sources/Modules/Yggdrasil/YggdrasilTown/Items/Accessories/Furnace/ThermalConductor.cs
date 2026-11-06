using Everglow.Commons.Mechanics.PlayerBonuses;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.Furnace;

public class ThermalConductor : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(
		40,
		8,
		StatBonusPlayer.ThermalConductorManaThreshold * 100f,
		StatBonusPlayer.ThermalConductorCheckFrames / 60f,
		StatBonusPlayer.ThermalConductorTriggerRate * 100f,
		StatBonusPlayer.ThermalConductorManaHeal,
		StatBonusPlayer.ThermalConductorDebuffDuration / 60f);

	public override void SetDefaults()
	{
		Item.width = 44;
		Item.height = 46;
		Item.accessory = true;
		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(gold: 2);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// 1. + 40 Mana
		// ============
		player.statManaMax2 += 40;

		// 2. + 8% Mana Cost
		// ==================
		player.manaCost += 0.08f;

		// 3. Mana recovery with a penalty
		// ============================================
		player.GetModPlayer<StatBonusPlayer>().ThermalConductor = true;
	}
}
