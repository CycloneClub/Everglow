using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.LightSeeker;

public class LightBeamStaff : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.width = 34;
		Item.height = 40;

		Item.damage = 12;
		Item.DamageType = DamageClass.Magic;
		Item.crit = 4;
		Item.knockBack = 3.5f;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useTime = 36;
		Item.useAnimation = 36;
		Item.UseSound = SoundID.Item12;
		Item.autoReuse = true;
		Item.noUseGraphic = true;
		Item.noMelee = true;
		Item.mana = 5;

		Item.value = ItemRarityID.Green;
		Item.value = Item.buyPrice(silver: 8);

		Item.shoot = ModContent.ProjectileType<LightBeamStaff_proj>();
		Item.shootSpeed = 15f;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
		return false;
	}
}
