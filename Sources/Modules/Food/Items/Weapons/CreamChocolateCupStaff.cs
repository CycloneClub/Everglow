using Everglow.Food.Projectiles;
using Mono.Cecil;
using Terraria.DataStructures;
using Terraria.ModLoader.Core;

namespace Everglow.Food.Items.Weapons;

public class CreamChocolateCupStaff : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetStaticDefaults()
	{
		Item.staff[Item.type] = true;
	}

	public override void SetDefaults()
	{
		Item.damage = 33;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 9;
		Item.width = 60;
		Item.height = 60;
		Item.useTime = 21;
		Item.useAnimation = 21;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.UseSound = SoundID.Item39.WithVolumeScale(0.8f) with { MaxInstances = 3 };
		Item.knockBack = 2.5f;
		Item.value = Item.sellPrice(0, 0, 20, 0);
		Item.rare = ItemRarityID.Green;
		Item.shoot = ModContent.ProjectileType<CreamChocolateCupStaff_proj>();
		Item.shootSpeed = 12f;
	}

	public override bool AltFunctionUse(Player player)
	{
		return true;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.altFunctionUse == 2)
		{
			Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CreamChocolateCupStaff_proj_rightClick>(), damage, knockback, player.whoAmI, 0, 0);
			return false;
		}
		if (player.ownedProjectileCounts[Item.shoot] < 1)
		{
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 0);
		}

		return false;
	}

	public override bool CanUseItem(Player player)
	{
		if (player.altFunctionUse == 2)
		{
			Item.UseSound = SoundID.Item132.WithVolumeScale(0.8f);
			Item.mana = 34;
		}
		else
		{
			Item.UseSound = SoundID.Item39.WithVolumeScale(0.8f) with { MaxInstances = 3 };
			Item.mana = 9;
		}
		return true;
	}

	public override void HoldItem(Player player)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<CreamChocolateCupStaff_proj_rightClick>()] + player.ownedProjectileCounts[ModContent.ProjectileType<CreamChocolateCupStaff_proj>()] + player.ownedProjectileCounts[ModContent.ProjectileType<CreamChocolateCupStaff_proj_held>()] == 0)
		{
			Projectile.NewProjectile(player.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<CreamChocolateCupStaff_proj_held>(), 0, 0, player.whoAmI, 0, 0);
		}
		base.HoldItem(player);
	}
}
