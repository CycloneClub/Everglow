using Everglow.Commons.MEAC;
using Everglow.Example.Projectiles;

namespace Everglow.Example.Items;

public class ExampleMeleeItem : MeleeItem_3D
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Examples;

	public override void SetCustomDefaults()
	{
		Item.width = 82;
		Item.damage = 200;
		Item.useTime = Item.useAnimation = 12;
		Item.shoot = ModContent.ProjectileType<ExampleMeleeProjectile>();
	}
}
