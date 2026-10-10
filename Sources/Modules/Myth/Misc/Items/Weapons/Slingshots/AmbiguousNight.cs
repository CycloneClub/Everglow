using Everglow.Commons.Templates.Weapons.Slingshots;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Items.Weapons.Slingshots;

public class AmbiguousNight : SlingshotItem
{
	public override void SetDef()
	{
		Item.damage = 54;
		Item.crit = 8;
		projType = ModContent.ProjectileType<Projectiles.Weapon.Ranged.Slingshots.AmbiguousNight>();
		Item.width = 40;
		Item.height = 32;
		Item.rare = ItemRarityID.Pink;
		Item.value = Item.sellPrice(0, 2, 0, 0);
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.ownedProjectileCounts[projType] < 1)
		{
			Projectile.NewProjectile(source, position, Vector2.Zero, projType, damage, knockback, player.whoAmI, Item.shootSpeed, Item.useAnimation);
		}
		return false;
	}
}
