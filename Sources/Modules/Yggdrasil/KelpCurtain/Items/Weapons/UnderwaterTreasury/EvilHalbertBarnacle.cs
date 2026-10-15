using Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee.EvilHalbertBarnacle;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons.UnderwaterTreasury;

public class EvilHalbertBarnacle : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeWeapons;

	public int State = 0;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 64;
		Item.height = 62;
		Item.useAnimation = 30;
		Item.useTime = 30;
		Item.shootSpeed = 10f;
		Item.knockBack = 5f;
		Item.damage = 46;
		Item.rare = ItemRarityID.Orange;

		Item.DamageType = DamageClass.Melee;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.value = 35000;
	}

	public override bool CanUseItem(Player player)
	{
		if (base.CanUseItem(player))
		{
			if (Main.myPlayer == player.whoAmI)
			{
				if (player.altFunctionUse != 2)
				{
					Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<EvilHalbertBarnacle_proj>(), player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
				}
				else
				{
					// Right Click
					if (player.ownedProjectileCounts[ModContent.ProjectileType<EvilHalbertBarnacle_proj_shuttle>()] <= 0)
					{
						State = 1;
						Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<EvilHalbertBarnacle_ShootShuttle>(), player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
					}
				}
			}
			return false;
		}
		return base.CanUseItem(player);
	}

	public override bool AltFunctionUse(Player player) => true;
}
