using Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Ammos;

public class HuskburstBullet : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Ammo;

	public override void SetDefaults()
	{
		Item.damage = 10;
		Item.ammo = AmmoID.Bullet;
		Item.consumable = true;
		Item.DamageType = DamageClass.Ranged;
		Item.width = 16;
		Item.height = 16;
		Item.value = 50;
		Item.maxStack = Item.CommonMaxStack;
		Item.shoot = ModContent.ProjectileType<HuskburstBullet_Proj>();
		Item.shootSpeed = 16f;
	}
}
