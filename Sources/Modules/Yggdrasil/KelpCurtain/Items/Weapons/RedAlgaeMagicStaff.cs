using Everglow.SpellAndSkull.Items;
using Everglow.Yggdrasil.KelpCurtain.Items.Materials;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class RedAlgaeMagicStaff : SpellTomeItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.damage = 54;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 12;
		Item.width = 46;
		Item.height = 46;
		Item.useTime = 30;
		Item.useAnimation = 30;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.knockBack = 2.7f;
		Item.value = 35000;
		Item.rare = ItemRarityID.Orange;
		Item.UseSound = SoundID.Item42;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<RedAlgaeMagicStaff_Proj>();
		Item.shootSpeed = 12f;
		Item.staff[Type] = true;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, -MathHelper.PiOver2);
		Projectile.NewProjectileDirect(source, position, velocity * 0.8f, type, damage, knockback, player.whoAmI, MathHelper.PiOver2);
		return false;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
		.AddIngredient(ModContent.ItemType<JadeLakeRedAlgae_Item>(), 12)
		.AddIngredient(ModContent.ItemType<CrimsonMoonSap>(), 1)
		.AddTile(TileID.WorkBenches)
		.Register();
	}
}
