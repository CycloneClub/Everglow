using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class PearShapedNeedle : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetStaticDefaults()
	{
		Item.staff[Type] = true;
	}

	public override void SetDefaults()
	{
		Item.width = 28;
		Item.height = 28;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 17;
		Item.knockBack = 2.5f;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useTime = Item.useAnimation = 12;
		Item.noUseGraphic = true;
		Item.noMelee = true;
		Item.autoReuse = false;

		Item.rare = ItemRarityID.Blue;
		Item.value = 4000;

		Item.shoot = ModContent.ProjectileType<PearShapedNeedle_HeldProj>();
		Item.shootSpeed = 8;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.ownedProjectileCounts[type] <= 0)
		{
			Projectile.NewProjectile(source, position, Vector2.zeroVector, type, damage, knockback, player.whoAmI);
		}
		return false;
	}
}
