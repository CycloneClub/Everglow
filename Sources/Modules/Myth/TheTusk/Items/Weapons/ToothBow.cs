using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Items.Weapons;

public class ToothBow : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 64;
		Item.height = 78;
		Item.rare = ItemRarityID.Green;

		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.autoReuse = false;
		Item.UseSound = SoundID.Item1;
		Item.channel = true;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 46;
		Item.knockBack = 5f;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.shoot = ProjectileID.WoodenArrowFriendly;
		Item.shootSpeed = 12f;
		Item.useAmmo = AmmoID.Arrow;
	}
	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Weapon.ToothBow>()] <= 0)
		{
			Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<Projectiles.Weapon.ToothBow>(), (int)(damage * 0.65f), knockback, player.whoAmI, type, Item.useAnimation);
		}

		return false;
	}
}
