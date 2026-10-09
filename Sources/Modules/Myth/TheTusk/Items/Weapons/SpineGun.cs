using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.Localization;

namespace Everglow.Myth.TheTusk.Items.Weapons;

public class SpineGun : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		// Common Properties
		Item.width = 64;
		Item.height = 40;
		Item.rare = ItemRarityID.Green;
		Item.value = 2000;

		// Use Properties
		Item.useTime = 30;
		Item.useAnimation = 30;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.autoReuse = true;
		Item.UseSound = SoundID.Item36;
		Item.noUseGraphic = true;

		// Weapon Properties
		Item.DamageType = DamageClass.Ranged;
		Item.damage = 15;
		Item.knockBack = 6f;
		Item.noMelee = true;

		// Gun Properties
		Item.shoot = ProjectileID.PurificationPowder;
		Item.shootSpeed = 18f;
		Item.useAmmo = AmmoID.Bullet;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Weapon.SpineGun>()] < 1)
		{
			Projectile.NewProjectileDirect(source, position + velocity * 2.0f - new Vector2(0, 4), Vector2.Zero, ModContent.ProjectileType<Projectiles.Weapon.SpineGun>(), damage, knockback, player.whoAmI, 1f, Item.useAnimation);
		}
		else
		{
			for (int x = 0; x < Main.projectile.Length; x++)
			{
				if (Main.projectile[x].active)
				{
					if (Main.projectile[x].type == ModContent.ProjectileType<Projectiles.Weapon.SpineGun>())
					{
						if (Main.projectile[x].owner == player.whoAmI)
						{
							Main.projectile[x].ai[0] = 1f;
							Main.projectile[x].ai[1] = Item.useAnimation;
						}
					}
				}
			}
		}
		Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(3));
		newVelocity *= 1f - Main.rand.NextFloat(0.1f);
		float Beilv = 1f;
		if (type == 242)
		{
			Beilv = 2.4f;
		}

		Projectile.NewProjectileDirect(source, position + newVelocity * 0.9f + new Vector2(0, -6), newVelocity * 2 * Beilv, ModContent.ProjectileType<Projectiles.Weapon.SplieSpineBullet>(), damage, knockback, player.whoAmI, player.GetCritChance(DamageClass.Ranged) + player.GetCritChance(DamageClass.Generic), type);
		return false;
	}
	public override Vector2? HoldoutOffset()
	{
		return new Vector2(-20f, -2f);
	}
}
