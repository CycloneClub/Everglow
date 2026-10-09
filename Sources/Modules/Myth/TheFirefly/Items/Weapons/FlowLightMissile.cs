using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Items.Weapons;

public class FlowLightMissile : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetStaticDefaults()
	{
		Item.staff[Item.type] = true;
	}

	public override void SetDefaults()
	{
		Item.damage = 33;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 4;
		Item.width = 60;
		Item.height = 60;
		Item.useTime = 21;
		Item.useAnimation = 21;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.UseSound = SoundID.Item132.WithVolumeScale(0.8f) with { MaxInstances = 3 };
		Item.knockBack = 2.5f;
		Item.value = Item.sellPrice(0, 0, 20, 0);
		Item.rare = ItemRarityID.Green;
		Item.shoot = ModContent.ProjectileType<Projectiles.FlowLightMissile>();
		Item.shootSpeed = 12f;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.ownedProjectileCounts[Item.shoot] < 1)
		{
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, 0);
		}

		return false;
	}
}
