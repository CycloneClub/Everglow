using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Items.Weapons;

public class GlowBeadGun : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.damage = 24;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 3;
		Item.width = 104;
		Item.height = 38;
		Item.useTime = 30;
		Item.useAnimation = 30;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.knockBack = 5f;
		Item.value = Item.sellPrice(0, 5, 0, 0);
		Item.rare = ItemRarityID.LightRed;
		Item.UseSound = SoundID.Item132 with { MaxInstances = 3 };
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<Projectiles.GlowBeadGun>();
		Item.shootSpeed = 8;
	}

	public override void HoldItem(Player player) => player.ListenMouseWorld();

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.ownedProjectileCounts[type] > 0)
		{
			foreach (Projectile projectile in Main.ActiveProjectiles)
			{
				if (player.ownedProjectileCounts[type] > 1)
				{
					if (projectile.type == type && projectile.owner == player.whoAmI)
					{
						projectile.Kill();
					}
				}
				else
				{
					break;
				}
			}
			return false;
		}
		return true;
	}
}
