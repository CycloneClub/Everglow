namespace Everglow.PlantAndFarm.Items.Accessories;

public class LightPurplePen : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Rabit Hair Calligraphy Brush");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "紫豪中山兔");
		// Tooltip.SetDefault("Increases damege, crit chance and melee speed by 7%\n'No rabit was hurt during the production process'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "伤害、暴击率和近战攻速各增加7%\n'没有任何兔子在制作过程中受到伤害'");
	}

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 36;
		Item.value = 3424;
		Item.accessory = true;
		Item.rare = 3;
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
			.AddTile(304)
			.Register();
	}
}
