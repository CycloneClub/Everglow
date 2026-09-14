namespace Everglow.Yggdrasil.KelpCurtain.Items.Accessories;

public class CorrodedPearl : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;

		Item.accessory = true;

		Item.value = Item.sellPrice(gold: 1);
		Item.rare = ItemRarityID.Green;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetDamage<SummonDamageClass>() += 0.05f;
		player.breathEffectiveness += 0.5f;
		player.GetModPlayer<KelpCurtainPlayer>().CorrodedPearl = true; // Increase max speed and acceleration by 20% when player is currently in water.
	}
}
