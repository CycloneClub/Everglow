using Everglow.Yggdrasil.KelpCurtain.Projectiles.Legacies;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons.Legacies;

public class BacterialAgent : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 24;
		Item.height = 30;
		Item.useAnimation = 30;
		Item.useTime = 30;

		Item.damage = 30;
		Item.rare = ItemRarityID.Green;

		Item.DamageType = DamageClass.Magic;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.shoot = ModContent.ProjectileType<BacterialAgent_proj>();
		Item.shootSpeed = 12f;
		Item.value = Item.sellPrice(gold: 1);
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		return true;
	}
}
