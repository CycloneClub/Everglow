using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Items.Weapons;

public class World : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.damage = 572;
		Item.DamageType = DamageClass.Melee;
		Item.width = 118;
		Item.height = 146;
		Item.useTime = 10;
		Item.useAnimation = 10;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.knockBack = 6;
		Item.value = 20000;
		Item.rare = ItemRarityID.Red;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<Projectiles.Weapon.Melee.World>();
		Item.shootSpeed = 0;
		Item.crit = 8;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.ownedProjectileCounts[type] == 0)
		{
			Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI, 2, 0f);
		}

		return false;
	}
}
