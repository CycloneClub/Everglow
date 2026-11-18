namespace Everglow.PlantAndFarm.Items.Accessories;

public class LightPurplePen : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 36;
		Item.value = 3424;
		Item.accessory = true;
		Item.rare = ItemRarityID.Orange;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetAttackSpeed(DamageClass.Generic) += 0.06f;
		player.GetDamage(DamageClass.Melee) *= 1.06f;
		player.GetCritChance(DamageClass.Melee) += 6;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<Materials.WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<Materials.Lavender>(), 24)
			.AddTile(TileID.LivingLoom)
			.Register();
	}
}
