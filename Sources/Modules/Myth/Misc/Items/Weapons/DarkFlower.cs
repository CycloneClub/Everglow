using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Items.Weapons;

public class DarkFlower : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.FlowerofFrost);
		Item.magic = true;
		Item.damage = 24;
		Item.mana = 7;
		Item.shootSpeed = 11;
		Item.useTime = 28;
		Item.useAnimation = 28;
		Item.useLimitPerAnimation = 1;
		Item.shoot = ModContent.ProjectileType<DarkFlower_Proj>();
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectile(source, position, velocity + Main.rand.NextVector2Circular(3, 3) + player.velocity * 0.5f, type, damage, knockback, player.whoAmI);
		return false;
	}
}
