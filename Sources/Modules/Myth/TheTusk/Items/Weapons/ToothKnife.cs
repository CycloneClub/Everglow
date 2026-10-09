using Everglow.Myth.TheTusk.Projectiles;
using Everglow.Myth.TheTusk.Projectiles.Weapon;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Items.Weapons;

public class ToothKnife : ModItem
{
	// TODO:暴击后在地上召唤獠牙刺
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	// TODO:暴击后在地上召唤獠牙刺
	public override void SetDefaults()
	{
		Item.width = 40;
		Item.height = 48;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.autoReuse = true;

		Item.DamageType = DamageClass.Melee;
		Item.damage = 25;
		Item.knockBack = 3.4f;
		Item.crit = 2;

		Item.value = 4050;
		Item.rare = ItemRarityID.Green;
		Item.UseSound = SoundID.Item1;

		Item.shoot = ModContent.ProjectileType<Projectiles.Weapon.ToothKnife>();
		Item.shootSpeed = 8f;
	}

	public override bool CanUseItem(Player player)
	{
		Item.useTime = (int)(18f / player.meleeSpeed);
		Item.useAnimation = (int)(18f / player.meleeSpeed);
		return player.ownedProjectileCounts[Item.shoot] < 1;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.ownedProjectileCounts[Item.shoot] < 1)
		{
			Projectile.NewProjectile(source, position + new Vector2(0, -24), velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
		}
		return false;
	}
}
