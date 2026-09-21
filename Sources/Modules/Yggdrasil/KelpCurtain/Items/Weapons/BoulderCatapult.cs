using Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class BoulderCatapult : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override string Texture => $"Terraria/Images/Item_{ItemID.GrenadeLauncher}";

	public override void SetDefaults()
	{
		Item.width = 60;
		Item.height = 36;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 44;
		Item.knockBack = 15f;

		Item.useTime = Item.useAnimation = 77;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.UseSound = SoundID.Item108;
		Item.autoReuse = true;

		Item.rare = ItemRarityID.Orange;
		Item.value = Item.buyPrice(gold: 2);

		Item.shoot = ModContent.ProjectileType<BoulderCatapult_Proj>();
		Item.shootSpeed = 12f;
	}
}
