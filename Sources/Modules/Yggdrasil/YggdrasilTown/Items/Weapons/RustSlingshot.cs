using Everglow.Commons.Templates.Weapons.Slingshots;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class RustSlingshot : SlingshotItem
{
	public override void SetDef()
	{
		Item.width = 30;
		Item.height = 30;

		Item.damage = 15;
		Item.knockBack = 1f;
		Item.crit = 42;

		Item.useTime = Item.useAnimation = 22;

		Item.value = Item.buyPrice(platinum: 0, gold: 1, silver: 23);
		Item.rare = ItemRarityID.Green;

		projType = ModContent.ProjectileType<RustSlingshot_Weapon>();
	}
}
