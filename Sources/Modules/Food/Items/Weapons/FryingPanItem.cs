using Everglow.Commons.MEAC;
using Everglow.Food.Projectiles;
using Terraria.DataStructures;

namespace Everglow.Food.Items.Weapons;

public class FryingPanItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 22;
		Item.useAnimation = 22;
		Item.width = 1;
		Item.height = 1;

		Item.knockBack = 5f;
		Item.damage = 16;
		Item.rare = ItemRarityID.Green;

		Item.DamageType = DamageClass.Melee;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.shootSpeed = 5f;
		Item.shoot = ModContent.ProjectileType<FryingPan>();

		Item.value = Item.sellPrice(gold: 2);
	}

	public override bool CanUseItem(Player player)
	{
		if (base.CanUseItem(player))
		{
			if (Main.myPlayer == player.whoAmI)
			{
				foreach (Projectile projectile in Main.projectile)
				{
					if (projectile.owner == player.whoAmI && projectile.type == ModContent.ProjectileType<FryingPan>() && projectile.active)
					{
						return false;
					}
				}
				if (player.altFunctionUse == 2)// 右键
				{
					Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<FryingPan>(), player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
					proj.scale *= Item.scale;
					(proj.ModProjectile as MeleeProj).isRightClick = true; // 指定为右键
					(proj.ModProjectile as MeleeProj).currantAttackType = 100; // 切换到弹幕的蓄力斩攻击方式
				}
				if (player.altFunctionUse != 2)
				{
					return true;
				}
			}
			return false;
		}
		return base.CanUseItem(player);
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.altFunctionUse != 2)// 左键
		{
			Projectile proj = Projectile.NewProjectileDirect(source, player.Center, velocity * 1.5f, type, damage, knockback * 2, player.whoAmI, 0f, 0f);
			proj.scale *= Item.scale;
		}

		return false;
	}

	public override bool AltFunctionUse(Player player)
	{
		return true;
	}

	public override void AddRecipes()
	{
	}
}
