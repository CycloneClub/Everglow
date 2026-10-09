using Everglow.Plant.Projectiles.Melee;

namespace Everglow.Plant.Items.Weapons.Melee;

public class CactusBall : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Rolling Cactus");
		// DisplayName.AddTranslation(PlantUtils.LocaizationChinese, "滚动的仙人掌");
		// Tooltip.SetDefault("Spin and throw out your cactus ball!\nShred enemy armour and make them bleed");
		// Tooltip.AddTranslation(PlantUtils.LocaizationChinese, "旋转并掷出你的仙人掌球！\n击碎敌人护甲，并让它们流血");
	}
	public override void SetDefaults()
	{
		Item.width = 50;
		Item.height = 50;
		Item.DamageType = DamageClass.Melee;
		Item.UseSound = SoundID.Item1;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noUseGraphic = true;
		Item.damage = 29;
		Item.knockBack = 3.75f;
		Item.useAnimation = Item.useTime = 30;
		Item.shoot = ModContent.ProjectileType<CactusBallProj>();
		Item.shootSpeed = 10f;
		Item.channel = true;
	}
	public override void AddRecipes()
	{
		CreateRecipe(1)
			.AddIngredient(ItemID.RollingCactus, 1)
			.AddIngredient(ItemID.PinkPricklyPear, 1)
			.AddIngredient(ItemID.Leather, 5)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
