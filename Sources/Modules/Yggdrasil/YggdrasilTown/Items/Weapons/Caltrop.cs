namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class Caltrop : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 18;
		Item.knockBack = 0.2f;
		Item.crit = 14;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = Item.useAnimation = 18;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = false;
		Item.noUseGraphic = true;
		Item.noMelee = true;

		Item.consumable = true;
		Item.maxStack = Item.CommonMaxStack;

		Item.value = Item.buyPrice(platinum: 0, gold: 0, silver: 0, copper: 23);
		Item.rare = ItemRarityID.White;

		Item.shootSpeed = 12;
		Item.shoot = ModContent.ProjectileType<Projectiles.Ranged.Caltrop>();
	}
}
