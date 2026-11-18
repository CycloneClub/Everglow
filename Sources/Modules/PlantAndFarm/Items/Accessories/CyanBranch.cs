namespace Everglow.PlantAndFarm.Items.Accessories;

public class CyanBranch : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 42;
		Item.value = 3142;
		Item.accessory = true;
		Item.rare = ItemRarityID.Orange;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// MythPlayer.CyanBranch = 2;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<Materials.WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<Materials.CyanHyacinth>(), 24)
			.AddTile(TileID.LivingLoom)
			.Register();
	}
}
