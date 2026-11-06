using Everglow.Yggdrasil.Common.Items;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.CyanVine;

public class CyanAmberStaff : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.staff[Type] = true;
		Item.width = 56;
		Item.height = 56;
		Item.useAnimation = 30;
		Item.useTime = 30;
		Item.knockBack = 2f;
		Item.damage = 18;
		Item.rare = ItemRarityID.White;
		Item.UseSound = SoundID.Item1;
		Item.value = 4000;
		Item.autoReuse = false;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 8;
		Item.noMelee = true;
		Item.shoot = ModContent.ProjectileType<AmberLiquidProj>();
		Item.shootSpeed = 6;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ModContent.ItemType<CyanVineStaff>())
			.AddIngredient(ModContent.ItemType<YggdrasilAmber>(), 3)
			.AddIngredient(ModContent.ItemType<YggdrasilGrayRock_Item>(), 20)
			.AddTile(TileID.Anvils)
			.Register();
	}

	public override bool CanUseItem(Player player)
	{
		return base.CanUseItem(player);
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectile(source, position + Vector2.Normalize(velocity) * 40, velocity, type, damage, knockback, player.whoAmI);
		return false;
	}
}
