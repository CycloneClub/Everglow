namespace Everglow.Yggdrasil.KelpCurtain.Items.Accessories;

public class GuppyShell : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;

		Item.defense = 2;

		Item.accessory = true;

		Item.value = Item.buyPrice(silver: 60);
		Item.rare = ItemRarityID.Blue;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.moveSpeed -= 0.05f; // Decrease movement speed by 5%.

		// When player is not moving
		if (player.IsConsideredStandingStill)
		{
			player.endurance += 0.1f; // Increase damage reduction by 10%.
			player.GetDamage<GenericDamageClass>() += 0.05f; // Increase damage by 5%.
		}
	}
}
