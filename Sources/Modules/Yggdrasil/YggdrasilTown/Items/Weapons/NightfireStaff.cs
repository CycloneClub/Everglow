using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class NightfireStaff : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.width = 22;
		Item.height = 22;

		Item.DamageType = DamageClass.Magic;
		Item.damage = 20;
		Item.knockBack = 0.5f;
		Item.mana = 7;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.UseSound = SoundID.Item20;
		Item.useTime = Item.useAnimation = 30;
		Item.noMelee = true;
		Item.autoReuse = false;

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(gold: 1);

		Item.shoot = ModContent.ProjectileType<NightfireStaff_Projectile>();
		Item.shootSpeed = 8;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Vector2 shootPos = Main.MouseWorld - 4 * velocity;
		Vector2 vel = shootPos - player.Center;
		if (vel.Length() > 256)
		{
			shootPos = player.Center + vel.NormalizeSafe() * 256f;
		}
		for (int j = 0; j < 8; j++)
		{
			for (int i = 0; i < 8; i++)
			{
				Dust dust = Dust.NewDustDirect(shootPos - new Vector2(4), 0, 0, ModContent.DustType<NightFire>());
				dust.velocity = new Vector2(0, MathF.Sqrt(i / 8f) * 12f).RotatedBy(Main.time + j * MathHelper.TwoPi / 8f + MathF.Sin(i));
				dust.scale = Main.rand.NextFloat(1.1f, 1.7f);
			}
		}
		Projectile.NewProjectile(source, shootPos + new Vector2(0, 20).RotatedBy(velocity.ToRotation() - MathHelper.Pi), velocity, type, damage, knockback, player.whoAmI, velocity.ToRotation(), 1);
		Projectile.NewProjectile(source, shootPos - new Vector2(0, 20).RotatedBy(velocity.ToRotation() - MathHelper.Pi), velocity, type, damage, knockback, player.whoAmI, velocity.ToRotation(), -1);
		return false;
	}
}
