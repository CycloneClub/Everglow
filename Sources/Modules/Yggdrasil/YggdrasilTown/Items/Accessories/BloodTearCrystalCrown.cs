using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class BloodTearCrystalCrown : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(30, 75, 0.5f);

	public override void SetDefaults()
	{
		Item.width = 38;
		Item.height = 30;
		Item.accessory = true;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(gold: 1, silver: 12);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// 1. + 30 Life
		player.statLifeMax2 += 30;

		// 2. + 1 life regen (if life is more than 75%)
		if (player.statLife > player.statLifeMax * 0.75f)
		{
			player.lifeRegen += 1;
		}
	}
}
