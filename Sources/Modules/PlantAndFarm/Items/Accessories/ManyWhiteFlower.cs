using Everglow.PlantAndFarm.Items.Materials;

namespace Everglow.PlantAndFarm.Items.Accessories;

public class ManyWhiteFlower : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Accessories;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Bouquet of Silk Star");
		// DisplayName.AddTranslation((int)GameCulture.CultureName.Chinese, "白星锦球");
		// Tooltip.SetDefault("Increases melee damege and speed by 8%\n'You don't want these lovely magical flowers to be dameged'");
		// Tooltip.AddTranslation((int)GameCulture.CultureName.Chinese, "近战攻速增加8%,近战伤害增加8%\n'你不想让这些神奇又可爱的花被损坏'");
	}

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 42;
		Item.value = 3024;
		Item.accessory = true;
		Item.rare = 3;
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetAttackSpeed(DamageClass.Generic) += 0.18f;
		// player.GetDamage(DamageClass.Melee).Additive += 0.08f;
	}

	/*public static void BoSilkStarSpeed(Projectile projectile)
        {
		projectile.velocity = (float)(projectile.velocity * 1.18f);
	}*/
	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<WindMoveSeed>(), 15)
			.AddIngredient(ModContent.ItemType<WhiteStar>(), 24)
			.AddTile(304)
			.Register();
	}
}
