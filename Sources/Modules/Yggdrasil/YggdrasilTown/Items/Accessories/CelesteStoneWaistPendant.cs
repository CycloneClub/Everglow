namespace Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;

public class CelesteStoneWaistPendant : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 24;
		Item.height = 44;
		Item.accessory = true;
		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(silver: 92);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// 1. + 20 Mana
		player.statManaMax2 += 20;

		// 2. + 3 mana regen (if mana is between 30% and 70%)
		if (player.statMana > player.statManaMax * 0.3f && player.statMana < player.statManaMax * 0.7f)
		{
			player.manaRegen += 3;
		}
	}
}
