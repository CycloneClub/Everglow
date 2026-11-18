namespace Everglow.PlantAndFarm.Items.Accessories;

public class WhitePedal : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 12;
		Item.value = 3020;
		Item.accessory = true;
		Item.rare = ItemRarityID.Orange;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetCritChance(DamageClass.Generic) += 4;
		// MythPlayer.WhitePedal = 2;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<Materials.WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<Materials.ShallowNight>(), 24)
			.AddTile(TileID.LivingLoom)
			.Register();
	}
}
