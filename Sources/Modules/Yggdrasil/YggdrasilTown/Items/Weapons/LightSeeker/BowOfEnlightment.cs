using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.LightSeeker;

public class BowOfEnlightment : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 64;
		Item.height = 78;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useAnimation = 26;
		Item.useTime = 26;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = false;
		Item.noUseGraphic = false;
		Item.noMelee = true;
		Item.channel = true;

		Item.damage = 20;
		Item.DamageType = DamageClass.Ranged;
		Item.crit = 4;
		Item.knockBack = 0f;

		Item.shoot = ProjectileID.WoodenArrowFriendly;
		Item.shootSpeed = 12f;
		Item.useAmmo = AmmoID.Arrow;

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(silver: 5);
	}

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
	{
		if (type == ProjectileID.WoodenArrowFriendly)
		{
			type = ModContent.ProjectileType<LightArrow>();
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
