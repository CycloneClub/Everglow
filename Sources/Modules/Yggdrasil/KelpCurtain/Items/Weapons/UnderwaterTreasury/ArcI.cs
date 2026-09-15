using Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury;

public class ArcI : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.RangedWeapons;

	public int ShootType = 0;

	public int Power = 0;

	public int CurrencyCount = 0;

	public override void SetDefaults()
	{
		Item.width = 90;
		Item.height = 20;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 39;
		Item.knockBack = 6f;
		Item.crit = 8;

		Item.noUseGraphic = true;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.UseSound = SoundID.Item40;
		Item.useTime = Item.useAnimation = 17;
		Item.noMelee = true;

		Item.rare = ItemRarityID.Orange;
		Item.value = 35000;

		Item.useAmmo = AmmoID.Bullet;
		Item.shoot = ProjectileID.Bullet;
		Item.shootSpeed = 16;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		ShootType = type;
		if (player.ownedProjectileCounts[ModContent.ProjectileType<ArcI_proj>()] <= 0)
		{
			Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ArcI_proj>(), damage, knockback, player.whoAmI);
		}
		return false;
	}

	public override void HoldItem(Player player)
	{
	}
}
