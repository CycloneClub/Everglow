using Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Ammos;

public class CyatheaArrow : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 18;
		Item.height = 32;

		Item.damage = 10;
		Item.DamageType = DamageClass.Ranged;
		Item.knockBack = 2f;

		Item.maxStack = Item.CommonMaxStack;
		Item.consumable = true;
		Item.ammo = AmmoID.Arrow;

		Item.rare = ItemRarityID.Blue;
		Item.value = 20;

		Item.shoot = ModContent.ProjectileType<CyatheaArrow_Proj>();
		Item.shootSpeed = 7f;
	}
}
