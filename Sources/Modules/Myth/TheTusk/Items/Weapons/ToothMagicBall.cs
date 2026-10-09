using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.Localization;

namespace Everglow.Myth.TheTusk.Items.Weapons;

public class ToothMagicBall : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Sanguine Orb");
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.damage = 24;
		Item.DamageType = DamageClass.Magic;
		Item.width = 18;
		Item.height = 18;
		Item.useTime = 30;
		Item.useAnimation = 30;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.knockBack = 6;
		Item.value = 2000;
		Item.rare = ItemRarityID.Green;
		Item.UseSound = SoundID.Item71;
		Item.autoReuse = true;
		Item.shoot = ModContent.ProjectileType<Projectiles.Weapon.ToothMagicBall>();
		Item.shootSpeed = 3;
		Item.crit = 1;
		Item.mana = 16;
	}
	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Weapon.ToothMagicBall>()] < 1)
		{
			return true;
		}

		return false;
	}
}
