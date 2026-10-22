using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.LightSeeker;

public class HandgunOfEnlightment : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 62;
		Item.height = 32;
		Item.scale = 0.75f;

		Item.useTime = 25;
		Item.useAnimation = 25;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.autoReuse = false;
		Item.UseSound = SoundID.Item41;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 15;
		Item.knockBack = 1f;
		Item.noMelee = true;

		Item.shoot = ModContent.ProjectileType<LightBullet>();
		Item.shootSpeed = 16f;
		Item.useAmmo = AmmoID.Bullet;

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(silver: 3);
	}

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
	{
		if (type == ProjectileID.Bullet)
		{
			type = ModContent.ProjectileType<LightBullet>();
		}
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<LightBulbOvule>(4);
		recipe.AddIngredient<CyanVineBar>(12);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
