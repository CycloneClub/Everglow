using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class SevenShotGun : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 48;
		Item.height = 28;

		Item.damage = 11;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 5f;
		Item.crit = 8;
		Item.noMelee = true;

		Item.useTime = Item.useAnimation = 54;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.UseSound = SoundID.Item41;
		Item.autoReuse = false;

		Item.rare = ItemRarityID.Blue;
		Item.value = 3776;

		Item.shootSpeed = 16f;
		Item.shoot = ProjectileID.Bullet;
		Item.useAmmo = AmmoID.Bullet;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		for (int i = 0; i < 7; i++)
		{
			Vector2 newVel = velocity.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(0.85f, 1.15f);
			Projectile.NewProjectileDirect(source, position, newVel, type, damage, knockback, player.whoAmI);
		}
		return false;
	}

	public override Vector2? HoldoutOffset() => new Vector2(-20, 0);
}
