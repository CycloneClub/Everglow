using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.CyanVine;

public class CyanVineFlintlock : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 68;
		Item.height = 26;
		Item.rare = ItemRarityID.Blue;
		Item.value = 2500;
		Item.useTime = 30;
		Item.useAnimation = 30;
		Item.damage = 35;
		Item.DamageType = DamageClass.Ranged;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.UseSound = SoundID.Item11;
		Item.autoReuse = true;
		Item.knockBack = 2f;
		Item.useAmmo = ModContent.ItemType<LampFruit>();
		Item.shoot = ModContent.ProjectileType<LampFruitCurrent>();
		Item.noMelee = true;
		Item.shootSpeed = 45f;
	}

	// 33%概率不消耗弹药
	public override void OnConsumeAmmo(Item ammo, Player player)
	{
		if (Main.rand.NextBool(3))
		{
			ammo.stack++;
		}
		base.OnConsumeAmmo(ammo, player);
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		return base.Shoot(player, source, position, velocity, type, damage, knockback);
	}

	public override Vector2? HoldoutOffset()
	{
		return new Vector2(-24f, 0);
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe();
		recipe.AddIngredient<CyanVineBar>(12);
		recipe.AddIngredient<LampWood_Wood>(20);
		recipe.AddIngredient<LampFruit>(25);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}
