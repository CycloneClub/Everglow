using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Items.Weapons;

public class Sunflower : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = 1;
		Item.shootSpeed = 9f;
		Item.shoot = ModContent.ProjectileType<Projectiles.Weapon.Melee.Sunflower>();
		Item.DamageType = DamageClass.Melee;
		Item.width = 46;
		Item.height = 46;
		Item.UseSound = SoundID.Item1;
		Item.useAnimation = 24;
		Item.useTime = 24;
		Item.noUseGraphic = true;
		Item.noMelee = true;
		Item.rare = 1;
		Item.damage = 8;
		Item.autoReuse = false;
		Item.knockBack = 2;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectile(source, position + velocity * 3f, velocity, type, damage, knockback, player.whoAmI, 0);
		return false;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Sunflower, 3)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
