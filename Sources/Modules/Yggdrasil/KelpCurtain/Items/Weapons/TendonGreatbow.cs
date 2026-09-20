using Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class TendonGreatbow : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 34;

		Item.DamageType = DamageClass.Ranged;
		Item.damage = 58;
		Item.knockBack = 8f;
		Item.crit = 12;

		Item.useTime = Item.useAnimation = 28;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.autoReuse = false;
		Item.UseSound = SoundID.Item5;

		Item.rare = ItemRarityID.Pink;
		Item.value = Item.buyPrice(gold: 4);

		Item.useAmmo = AmmoID.Arrow;
		Item.shoot = ProjectileID.WoodenArrowFriendly;
		Item.shootSpeed = 12f;
	}

	public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
	{
		type = ModContent.ProjectileType<TendonGreatbow_Arrow>();
	}
}
