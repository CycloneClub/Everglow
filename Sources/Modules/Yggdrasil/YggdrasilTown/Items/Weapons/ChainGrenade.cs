using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class ChainGrenade : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 30;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 40;
		Item.knockBack = 1.0f;
		Item.crit = 4;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useAnimation = Item.useTime = 36;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = false;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.rare = ItemRarityID.Blue;
		Item.value = 4000;

		Item.shoot = ModContent.ProjectileType<ChainGrenadeProj>();
		Item.shootSpeed = 6;
	}
}
