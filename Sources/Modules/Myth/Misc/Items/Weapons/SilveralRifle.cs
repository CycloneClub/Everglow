using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Items.Weapons;

public class SilveralRifle : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.damage = 32;
		Item.DamageType = DamageClass.Ranged;
		Item.width = 66;
		Item.height = 24;
		Item.useTime = 40;
		Item.useAnimation = 40;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.knockBack = 0;
		Item.value = 500;
		Item.rare = ItemRarityID.Blue;
		Item.UseSound = SoundID.Item11;
		Item.autoReuse = true;
		Item.shoot = ProjectileID.PurificationPowder;
		Item.shootSpeed = 24f;
		Item.useAmmo = AmmoID.Bullet;
		Item.crit = 16;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectile(source, position + velocity * 2 + new Vector2(0, -2), velocity, type, damage, knockback, player.whoAmI, 0);
		return false;
	}

	public override Vector2? HoldoutOffset()
	{
		return new Vector2(-6f, 0);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.SilverBar, 12)
			.AddIngredient(ItemID.Ruby, 4)
			.AddTile(TileID.Anvils)
			.Register();
		CreateRecipe()
			.AddIngredient(ItemID.TungstenBar, 12)
			.AddIngredient(ItemID.Ruby, 4)
			.AddTile(TileID.Anvils)
			.Register();
	}
}
