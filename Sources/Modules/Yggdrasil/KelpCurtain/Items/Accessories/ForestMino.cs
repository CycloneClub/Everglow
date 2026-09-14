namespace Everglow.Yggdrasil.KelpCurtain.Items.Accessories;

public class ForestMino : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 40;
		Item.height = 48;

		Item.value = Item.sellPrice(gold: 2);
		Item.rare = ItemRarityID.Green;

		Item.defense = 4;

		Item.accessory = true;
	}

	public override void UpdateEquip(Player player)
	{
		// When in the Kelp Curtain biome.
		if (player.InModBiome<KelpCurtainBiome>())
		{
			player.moveSpeed += 0.25f; // Increases movement speed by 25%.
			player.aggro -= 400; // Reduces aggro by 400.
		}
	}
}
