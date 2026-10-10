using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class YellowBellFlower : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Bananea of the Valley");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "金簇铃兰");
		// Tooltip.SetDefault("Increases damage by (defense * 20%)%\n'Rare and beautiful'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "伤害增加(20%防御力)%\n'稀有又好看'");
	}

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
		player.GetDamage(DamageClass.Generic) *= player.statDefense * 0.2f / 100f + 1;
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
