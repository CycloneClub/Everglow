using Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class TendonGreatbow : ModItem
{
	public const int ChargeDuration = 28;

	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override string Texture => $"Terraria/Images/Item_{ItemID.Marrow}";

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 34;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 58;
		Item.knockBack = 8f;
		Item.crit = 12;

		Item.useTime = Item.useAnimation = 28;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.channel = true;
		Item.autoReuse = false;

		Item.rare = ItemRarityID.Pink;
		Item.value = Item.buyPrice(gold: 4);

		Item.useAmmo = AmmoID.Arrow;
		Item.shoot = ProjectileID.WoodenArrowFriendly;
		Item.shootSpeed = 12f;
	}

	public static bool ShouldRelease(int chargeFrames, bool channeling) => !channeling && chargeFrames >= ChargeDuration;

	public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<TendonGreatbowHeld>()] == 0;

	// The initial use only raises the bow. PickAmmo consumes ammunition on release.
	public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<TendonGreatbowHeld>()] > 0;

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.whoAmI == Main.myPlayer)
		{
			Projectile.NewProjectile(source, player.MountedCenter, velocity.SafeNormalize(Vector2.UnitX * player.direction),
				ModContent.ProjectileType<TendonGreatbowHeld>(), damage, knockback, player.whoAmI);
		}
		return false;
	}
}
