namespace Everglow.PlantAndFarm.Items.Accessories;

public class PurpleBall : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 24;
		Item.value = 3226;
		Item.accessory = true;
		Item.rare = ItemRarityID.Orange;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		// MythPlayer.PurpleBallFlower = 2;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<Materials.WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<Materials.LightPurpleBalls>(), 24)
			.AddTile(TileID.LivingLoom)
			.Register();
	}
}
