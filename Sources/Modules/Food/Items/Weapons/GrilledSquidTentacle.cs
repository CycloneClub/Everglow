using Everglow.Commons.Templates.Weapons.Whips;
using Everglow.Food.Projectiles;

namespace Everglow.Food.Items.Weapons;

public class GrilledSquidTentacle : WhipItem
{
	public override void SetDef()
	{
		Item.width = 18;
		Item.height = 18;
		Item.shoot = ModContent.ProjectileType<GrilledSquidTentacle_Proj>();
		Item.shootSpeed = 5.04f;
		Item.value = 32000;
		Item.rare = ItemRarityID.White;
		Item.damage = 25;
		Item.useAnimation = 30;
		Item.useTime = 30;
	}
}
