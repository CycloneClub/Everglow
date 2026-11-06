using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class StoneJavelin : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		// Common Properties
		Item.rare = ItemRarityID.White;
		Item.value = Item.sellPrice(copper: 1);
		Item.maxStack = Item.CommonMaxStack;

		// Use Properties
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useAnimation = Item.useTime = 18;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.consumable = true;

		// Weapon Properties
		Item.DamageType = DamageClass.Ranged;
		Item.damage = 12;
		Item.knockBack = 5f;
		Item.crit = 4;
		Item.noUseGraphic = true;
		Item.noMelee = true;

		// Projectile Properties
		Item.shootSpeed = 12f;
		Item.shoot = ModContent.ProjectileType<StoneJavelinProjectile>();
	}
}
