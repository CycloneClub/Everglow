using Everglow.Commons.Templates.Weapons.Whips;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Legacies;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons.Legacies;

public class LichenTentacle : WhipItem
{
	public override void SetDef()
	{
		Item.width = 40;
		Item.height = 32;
		Item.shoot = ModContent.ProjectileType<LichenTentacle_Proj>();
		Item.shootSpeed = 5.04f;
		Item.value = 7000;
		Item.rare = ItemRarityID.White;
		Item.damage = 25;
		Item.useAnimation = 30;
		Item.useTime = 30;
	}
}
