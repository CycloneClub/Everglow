using Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class BoulderCatapult : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

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

		// The design cell 不消耗子弹 is read literally: this launcher fires its own
		// boulder projectile and therefore sets no Item.useAmmo.
		Item.shoot = ModContent.ProjectileType<BoulderCatapult_Proj>();
		Item.shootSpeed = 12f;
	}
}
