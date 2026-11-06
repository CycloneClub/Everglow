using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.CyanVine;

public class CyanVineBow : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 34;
		Item.rare = ItemRarityID.White;
		Item.noMelee = true;
		Item.DamageType = DamageClass.Ranged;
		Item.UseSound = SoundID.Item5;
		Item.autoReuse = false;
		Item.value = 1200;
		Item.useTime = 26;
		Item.useAnimation = 26;
		Item.damage = 11;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.shoot = ProjectileID.WoodenArrowFriendly;
		Item.shootSpeed = 12f;
		Item.useAmmo = AmmoID.Arrow;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		return base.Shoot(player, source, position, velocity, type, damage, knockback);
	}

	public override Vector2? HoldoutOffset()
	{
		return new Vector2(0, 0);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<CyanVineBar>(7);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
