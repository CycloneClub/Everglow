using Everglow.Commons.Templates.Weapons.Yoyos;

namespace Everglow.Myth.TheTusk.Items.Weapons;

public class BloodyBoneYoyo : YoyoItem
{
	public override void SetCustomDefaults()
	{
		Item.shoot = ModContent.ProjectileType<Projectiles.Weapon.BloodyBoneYoyo>();
		Item.knockBack = 0.2f;
		Item.damage = 21;
		Item.value = Item.sellPrice(0, 0, 2, 0);
		Item.rare = ItemRarityID.Green;
	}
}
