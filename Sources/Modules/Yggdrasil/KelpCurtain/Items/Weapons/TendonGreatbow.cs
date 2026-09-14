using Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class TendonGreatbow : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedWeapons;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
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

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (type == ProjectileID.WoodenArrowFriendly)
		{
			type = ModContent.ProjectileType<TendonGreatbow_Arrow>();
		}
		return true;
	}

	// The design 合成方式 cell lists 血云母 + 血肉聚合物 + 玉化龙骨. All three are Phase 7
	// Giant Winged Dragon items that do not exist in the repository, and an item-type
	// reference to an absent type does not compile, so no AddRecipes body is written here;
	// the recipe is recorded as a precise blocker instead.
	//
	// The design also documents 按住左键蓄力拉出大弓 with no charge time, damage curve or release
	// behaviour, so the charge clause is recorded as an effect blocker and is not implemented.
}
