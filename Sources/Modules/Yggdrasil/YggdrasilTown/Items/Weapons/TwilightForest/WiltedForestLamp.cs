using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;

public class WiltedForestLamp : ModItem
{
	// Wilted Forest Lamp will not occupy minion slots, but can only exist 8 per player.
	public override string LocalizationCategory => LocalizationUtils.Categories.SummonWeapons;

	// Wilted Forest Lamp will not occupy minion slots, but can only exist 8 per player.
	public override void SetDefaults()
	{
		Item.width = 48;
		Item.height = 48;

		Item.DamageType = DamageClass.Summon;
		Item.damage = 14;
		Item.knockBack = 0.5f;
		Item.mana = 14;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.UseSound = SoundID.Item20;
		Item.useTime = Item.useAnimation = 32;
		Item.noMelee = true;
		Item.autoReuse = false;
		Item.rare = ItemRarityID.Green;
		Item.value = 14400;

		Item.shoot = ModContent.ProjectileType<WiltedForestLamp_Proj>();
		Item.shootSpeed = 8;
	}

	public override bool CanUseItem(Player player)
	{
		if (player.ownedProjectileCounts[Item.shoot] >= 8)
		{
			return false;
		}
		return base.CanUseItem(player);
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
		return false;
	}
}
