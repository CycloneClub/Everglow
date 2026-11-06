using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class DarkMassacreDagger : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 40;
		Item.height = 40;
		Item.useAnimation = 12;
		Item.useTime = 12;
		Item.knockBack = 1.1f;
		Item.damage = 24;
		Item.rare = ItemRarityID.White;
		Item.autoReuse = true;
		Item.DamageType = DamageClass.Melee;
		Item.noUseGraphic = true;
		Item.noMelee = true;
		Item.shootSpeed = 5f;
		Item.shoot = ModContent.ProjectileType<DarkMassacreDagger_Projectile>();
		Item.value = 2320;
	}

	public override bool CanUseItem(Player player)
	{
		Item.useTime = (int)(18f / player.meleeSpeed);
		Item.useAnimation = (int)(18f / player.meleeSpeed);
		return player.ownedProjectileCounts[Item.shoot] < 1;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.altFunctionUse == 2)
		{
			Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DarkMassacreDagger_Projectile_thrust>(), damage, knockback, player.whoAmI, 0f, 0f);
		}
		else if (player.ownedProjectileCounts[Item.shoot] < 1)
		{
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
		}
		return false;
	}

	public override bool AltFunctionUse(Player player)
	{
		return true;
	}

	public override bool? UseItem(Player player)
	{
		return null;
	}
}
