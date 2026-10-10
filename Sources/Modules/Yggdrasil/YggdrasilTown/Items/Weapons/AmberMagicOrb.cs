using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class AmberMagicOrb : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 40;
		Item.height = 52;
		Item.useAnimation = 17;
		Item.useTime = 17;
		Item.knockBack = 2f;
		Item.damage = 5;
		Item.rare = ItemRarityID.Blue;
		Item.UseSound = SoundID.Item1;
		Item.value = 4514;
		Item.autoReuse = false;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 18;
		Item.noUseGraphic = true;
		Item.autoReuse = true;
		Item.noMelee = true;

		Item.shoot = ModContent.ProjectileType<Projectiles.Magic.AmberMagicOrb>();
		Item.shootSpeed = 12;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.ownedProjectileCounts[type] <= 0)
		{
			Projectile p0 = Projectile.NewProjectileDirect(source, position + velocity * 5, velocity, type, damage, knockback, player.whoAmI);
			var amberMagicOrb = p0.ModProjectile as Projectiles.Magic.AmberMagicOrb;
			if (amberMagicOrb != null)
			{
				amberMagicOrb.ShootProj(velocity);
			}
		}
		else
		{
			foreach (Projectile proj in Main.projectile)
			{
				if (proj.active && proj.owner == player.whoAmI)
				{
					if (proj.type == type)
					{
						var amberMagicOrb = proj.ModProjectile as Projectiles.Magic.AmberMagicOrb;
						if (amberMagicOrb != null)
						{
							amberMagicOrb.ShootProj(velocity);
						}
					}
				}
			}
		}
		return false;
	}
}
