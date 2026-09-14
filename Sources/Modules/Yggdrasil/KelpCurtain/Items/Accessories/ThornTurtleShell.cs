namespace Everglow.Yggdrasil.KelpCurtain.Items.Accessories;

public class ThornTurtleShell : ModItem
{

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;

		Item.defense = 6;

		Item.accessory = true;

		Item.value = Item.buyPrice(gold: 1);
		Item.rare = ItemRarityID.Blue;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.AddBuff(BuffID.Thorns, 2);
		player.endurance += 0.05f; // Increase damage reduction by 5%.
		player.moveSpeed -= 0.07f; // Decrease movement speed by 7%.
		player.maxRunSpeed *= 1 - 0.05f; // Decrease run speed by 5%.
	}
}
