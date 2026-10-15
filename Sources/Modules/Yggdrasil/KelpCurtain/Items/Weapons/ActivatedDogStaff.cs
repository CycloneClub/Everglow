using Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class ActivatedDogStaff : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.SummonWeapons;

	public const int LeftManaCost = 10;
	public const int RightManaCost = 20;
	public const float SpecialProjectileSpeed = 15f;

	public override void SetStaticDefaults()
	{
		Item.staff[Type] = true;
	}

	public override void SetDefaults()
	{
		Item.DamageType = DamageClass.Summon;
		Item.damage = 8;
		Item.knockBack = 1.1f;
		Item.mana = LeftManaCost;

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = Item.useAnimation = 20;
		Item.noMelee = true;

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(gold: 1);

		Item.shoot = ModContent.ProjectileType<Wither_Activated_Dog_Summon>();
		Item.shootSpeed = 12f;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		Projectile proj = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.zeroVector, type, damage, knockback, player.whoAmI);
		Projectile proj2 = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.zeroVector, ModContent.ProjectileType<Wither_Activated_Dog_Summon_background>(), damage, knockback, player.whoAmI);
		proj.spriteDirection = -1;
		proj2.spriteDirection = -1;
		if (Main.MouseWorld.X < player.Center.X)
		{
			proj.spriteDirection = 1;
			proj2.spriteDirection = 1;
		}
		return false;
	}

	public override bool CanUseItem(Player player)
	{
		if (player.maxMinions - player.GetSlotsMinions() < 1)
		{
			return false;
		}
		return base.CanUseItem(player);
	}
}
