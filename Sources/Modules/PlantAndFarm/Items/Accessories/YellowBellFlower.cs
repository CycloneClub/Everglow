using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Utilities;
using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class YellowBellFlower : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 26;
		Item.height = 34;
		Item.value = 3229;
		Item.accessory = true;
		Item.rare = ItemRarityID.Orange;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetModPlayer<StatBonusPlayer>().YellowBellFlower = true;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<GoldWhip>(), 24)
			.AddTile(TileID.LivingLoom)
			.Register();
	}
}
