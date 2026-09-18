using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;

public class SpringOfQuicksand : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MagicWeapons;

	public override void SetStaticDefaults()
	{
		Item.staff[Type] = true;
	}

	public override void SetDefaults()
	{
		Item.width = 48;
		Item.height = 48;

		Item.DamageType = DamageClass.Magic;
		Item.damage = 16;
		Item.knockBack = 0.5f;
		Item.mana = 7;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.UseSound = SoundID.Item20;
		Item.useTime = Item.useAnimation = 32;
		Item.noMelee = true;
		Item.autoReuse = false;
		Item.rare = ItemRarityID.Green;
		Item.value = 14400;

		Item.shoot = ModContent.ProjectileType<SpringOfQuicksand_Sandflow>();
		Item.shootSpeed = 8;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectile(source, position, velocity * 1.5f, type, damage, knockback, player.whoAmI);
		return false;
	}
}
