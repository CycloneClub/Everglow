using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;

public class QuenchingBlade : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.width = 48;
		Item.height = 58;

		Item.DamageType = DamageClass.Melee;
		Item.damage = 27;
		Item.knockBack = 35;

		Item.useStyle = ItemUseStyleID.Shoot;
		Item.UseSound = SoundID.Item1;
		Item.useTime = Item.useAnimation = 28;
		Item.autoReuse = true;
		Item.noUseGraphic = true;
		Item.noMelee = true;

		Item.rare = ItemRarityID.Blue;
		Item.value = Item.buyPrice(gold: 3);

		Item.shoot = ModContent.ProjectileType<QuenchingBladeProj>();
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.ownedProjectileCounts[type] <= 0)
		{
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
		}
		return false;
	}
}
