using Everglow.Commons.Templates.Weapons.Whips;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class GreenVineWhip : WhipItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.SummonWeapons;

	public override void SetDef()
	{
		Item.width = 40;
		Item.height = 32;
		Item.shoot = ModContent.ProjectileType<GreenVineWhip_proj>();
		Item.shootSpeed = 5.04f;
		Item.value = 10000;
		Item.rare = ItemRarityID.Orange;
		Item.damage = 16;
		Item.useAnimation = 30;
		Item.useTime = 30;
	}
}
