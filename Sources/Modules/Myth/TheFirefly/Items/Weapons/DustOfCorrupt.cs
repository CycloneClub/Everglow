using System;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Items.Weapons;

public class DustOfCorrupt : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.damage = 13;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 4;
		Item.width = 60;
		Item.height = 60;
		Item.useTime = 3;
		Item.useAnimation = 3;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noUseGraphic = true;
		Item.noMelee = true;
		Item.knockBack = 2.5f;
		Item.value = Item.sellPrice(0, 0, 20, 0);
		Item.rare = ItemRarityID.Green;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<Projectiles.CorruptDust>();
		Item.shootSpeed = 17f;
	}

	public override void SetStaticDefaults()
	{
		Item.staff[Item.type] = true;
	}

	private Projectile staff = null;

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.StaffOfCorruptDust>()] < 1)
		{
			staff = Projectile.NewProjectileDirect(source, position + Vector2.Normalize(velocity) * 48, velocity, ModContent.ProjectileType<Projectiles.StaffOfCorruptDust>(), 0, knockback, player.whoAmI);
			Projectile.NewProjectile(source, position + Vector2.Normalize(velocity) * 48, velocity, type, damage, knockback, player.whoAmI, Math.Max(1f, player.velocity.Length() / 4f), player.GetCritChance(DamageClass.Magic));
		}
		else
		{
			Vector2 v0 = new Vector2(1, 0).RotatedBy(staff.rotation) * velocity.Length();
			Projectile.NewProjectile(source, staff.Center + Vector2.Normalize(v0) * 10, v0, type, damage, knockback, player.whoAmI, Math.Max(1f, player.velocity.Length() / 4f), player.GetCritChance(DamageClass.Magic));
		}
		return false;
	}
}
