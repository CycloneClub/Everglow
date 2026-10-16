namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.Auburn;

public class RodSpear : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.width = 50;
		Item.height = 54;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useAnimation = 45;
		Item.useTime = 45;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = true;
		Item.noUseGraphic = true;
		Item.noMelee = true;

		Item.damage = 18;
		Item.DamageType = DamageClass.Melee;
		Item.crit = 4;
		Item.knockBack = 4f;

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(silver: 20);

		Item.shoot = ModContent.ProjectileType<Projectiles.Melee.RodSpear>();
		Item.shootSpeed = 12;
	}
}
