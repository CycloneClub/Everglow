using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.TwilightForest;

/// <summary>
/// 机关长戟Item类
/// </summary>
public class MechanismHalberd : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 64;
		Item.height = 64;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 22;
		Item.useAnimation = 22;
		Item.damage = 26;
		Item.knockBack = 1.3f;
		Item.crit = 4;
		Item.DamageType = DamageClass.Melee;
		Item.value = 14400;
		Item.rare = ItemRarityID.Green;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.shootSpeed = 1f;
	}

	public override bool CanUseItem(Player player)
	{
		if (base.CanUseItem(player))
		{
			if (Main.myPlayer == player.whoAmI)
			{
				if (player.altFunctionUse != 2)
				{
					Projectile.NewProjectile(
						player.GetSource_ItemUse(Item),
						player.Center, Vector2.Zero,
						ModContent.ProjectileType<MechanismHalberd_Proj>(),
						player.GetWeaponDamage(Item),
						Item.knockBack, player.whoAmI);
				}
			}
			return false;
		}
		return base.CanUseItem(player);
	}
}
