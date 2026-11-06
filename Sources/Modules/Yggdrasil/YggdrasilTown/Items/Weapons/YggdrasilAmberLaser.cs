using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class YggdrasilAmberLaser : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.width = 38;
		Item.height = 44;

		Item.DamageType = DamageClass.Magic;
		Item.damage = 16;
		Item.knockBack = 4f;
		Item.mana = 12;

		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(silver: 31, copper: 6);

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useAnimation = 42;
		Item.useTime = 42;
		Item.UseSound = SoundID.Item1;
		Item.autoReuse = false;
		Item.channel = true;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.shoot = ModContent.ProjectileType<YggdrasilAmberLaser_proj>();
		Item.shootSpeed = 12;
	}
}
