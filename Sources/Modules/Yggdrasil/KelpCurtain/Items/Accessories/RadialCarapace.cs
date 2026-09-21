namespace Everglow.Yggdrasil.KelpCurtain.Items.Accessories;

public class RadialCarapace : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override string Texture => $"Terraria/Images/Item_{ItemID.FrozenTurtleShell}";

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;

		Item.defense = 4;

		Item.accessory = true;

		Item.value = Item.buyPrice(gold: 1);
		Item.rare = ItemRarityID.Green;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.statDefense += 4; // Increase defense by 4.
		player.moveSpeed -= 0.05f; // Decrease movement speed by 5%.
		player.GetModPlayer<KelpCurtainPlayer>().RadialCarapace = true; // Increase max speed and acceleration by 35% when player is currently in water.
	}
}
