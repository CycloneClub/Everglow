using Terraria.DataStructures;

namespace Everglow.Commons.Templates.Weapons.CrossBow;

public abstract class CrossBowItem : ModItem
{
	public override string LocalizationCategory => Utilities.LocalizationUtils.Categories.RangedWeapons;

	public int CrossBowProjType = -1;
	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.autoReuse = true;

		Item.DamageType = DamageClass.Ranged;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.shoot = ProjectileID.WoodenArrowFriendly;
		Item.shootSpeed = 14f;
		Item.useAmmo = AmmoID.Arrow;
		SetDef();
	}

	public virtual void SetDef()
	{
	}
	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (CrossBowProjType == -1)
		{
			return false;
		}
		var p0 = Projectile.NewProjectileDirect(source, position, Vector2.zeroVector, CrossBowProjType, damage, knockback, player.whoAmI);
		var crossBowProjectile = p0.ModProjectile as CrossBowProjectile;
		crossBowProjectile.ShootProjType = type;
		return false;
	}
}
