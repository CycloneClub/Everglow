using Everglow.Myth.Misc.Projectiles.Weapon.Melee.PrimordialJadeWinged_Spear;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.Acytaea.Vanities;

public class Acytaea_sword_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.rare = ItemRarityID.Purple;
		Item.value = 999999;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useAnimation = 18;
		Item.useTime = 18;
		Item.autoReuse = true;
		Item.damage = 120;
		Item.crit = 22;
		Item.knockBack = 6.5f;
		Item.noUseGraphic = true;
		Item.DamageType = DamageClass.Melee;
		Item.noMelee = true;
		Item.shootSpeed = 5f;
		Item.shoot = ModContent.ProjectileType<Projectiles.AcytaeaSword_projectile>();
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
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
		}
		return false;
	}

	public override bool? UseItem(Player player)
	{
		if (!Main.dedServ)
		{
			SoundEngine.PlaySound(Item.UseSound, player.Center);
		}

		return null;
	}
}
