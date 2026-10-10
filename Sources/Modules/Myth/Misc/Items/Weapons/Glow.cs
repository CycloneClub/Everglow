using Everglow.Myth.Misc.Projectiles.Weapon.Melee;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Items.Weapons
{
	public class Glow : ModItem
	{
		public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

		public override void SetDefaults()
		{
			Item.damage = 174;
			Item.DamageType = DamageClass.Melee;
			Item.width = 52;
			Item.height = 52;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.value = 90000;
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<Projectiles.Weapon.Melee.Glow>();
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shootsEveryUse = true;
			Item.autoReuse = true;
			Item.crit = 16;
		}

		public int UseCount = 0;

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			UseCount++;
			float adjustedItemScale = player.GetAdjustedItemScale(Item); // Get the melee scale of the player and item.
			Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
			velocity = Vector2.Normalize(Main.MouseWorld - player.MountedCenter) * 30f;
			Projectile.NewProjectile(source, player.MountedCenter, velocity, ModContent.ProjectileType<GlowMoonBlade>(), damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
			NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI); // Sync the changes in multiplayer.
			if (UseCount >= 3)
			{
				UseCount = 0;
				Vector2 p1 = new Vector2(Main.rand.NextFloat(-300, 300), -1000);
				velocity = Vector2.Normalize(Main.MouseWorld - p1 - player.MountedCenter) * 60f;
				Projectile.NewProjectileDirect(source, player.MountedCenter + p1, velocity, ModContent.ProjectileType<Glow_Fall>(), damage * 3, knockback, player.whoAmI, player.direction * player.gravDir, adjustedItemScale);
				Vector2 p2 = p1 + new Vector2(300 * player.direction, 0);
				velocity = Vector2.Normalize(Main.MouseWorld - p2 - player.MountedCenter) * 60f;
				Projectile.NewProjectileDirect(source, player.MountedCenter + p2, velocity, ModContent.ProjectileType<Glow_Fall>(), damage * 3, knockback, player.whoAmI, player.direction * player.gravDir, adjustedItemScale);
			}
			return base.Shoot(player, source, position, velocity, type, damage, knockback);
		}
	}
}
