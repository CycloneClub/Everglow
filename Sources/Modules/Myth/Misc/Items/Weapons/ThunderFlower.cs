using Everglow.Myth.Misc.Projectiles.Weapon.Magic;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Items.Weapons;

public class ThunderFlower : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.damage = 75;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 15;
		Item.width = 28;
		Item.height = 28;
		Item.useTime = 30;
		Item.useAnimation = 30;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.staff[Item.type] = true;
		Item.noMelee = true;
		Item.knockBack = 5f;
		Item.value = Item.sellPrice(0, 2, 0, 0);
		Item.rare = ItemRarityID.LightPurple;
		Item.UseSound = SoundID.Item43;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<ThunderBall>();
		Item.shootSpeed = 12f;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, player.GetCritChance(DamageClass.Magic) + player.GetCritChance(DamageClass.Generic));
		return false;
	}
}
