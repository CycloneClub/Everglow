using Everglow.Commons.Templates.Weapons.Slingshots;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.RockElemental;

public class BrittleRockSlingshot : SlingshotItem
{
	public override void SetDef()
	{
		Item.damage = 24;
		Item.knockBack = 10;
		projType = ModContent.ProjectileType<BrittleRockSlingshotProj>();
		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(0, 0, 55, 0);
	}
}
