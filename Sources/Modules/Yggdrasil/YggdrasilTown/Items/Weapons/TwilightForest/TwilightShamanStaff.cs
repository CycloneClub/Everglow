namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;

public class TwilightShamanStaff : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.width = 36;
		Item.height = 34;

		Item.DamageType = DamageClass.Magic;
		Item.damage = 30;
		Item.knockBack = 0.4f;
		Item.crit = 4;

		Item.useTime = Item.useAnimation = 15;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.UseSound = SoundID.Item20;
		Item.autoReuse = true;
		Item.noMelee = true;

		Item.shoot = ProjectileID.BallofFrost;
		Item.shootSpeed = 10;
	}
}
