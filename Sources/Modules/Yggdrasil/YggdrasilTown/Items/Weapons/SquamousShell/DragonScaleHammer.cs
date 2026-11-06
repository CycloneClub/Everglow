using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.SquamousShell;

public class DragonScaleHammer : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 68;
		Item.height = 72;
		Item.useAnimation = 5;
		Item.useTime = 5;
		Item.shoot = ModContent.ProjectileType<DragonScaleHammerProj>();
		Item.shootSpeed = 5f;
		Item.knockBack = 3.3f;
		Item.damage = 42;

		Item.DamageType = DamageClass.Melee;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.hammer = 50;

		Item.rare = ItemRarityID.Green;
		Item.value = Item.buyPrice(silver: 89);
	}

	public override bool CanUseItem(Player player)
	{
		if (base.CanUseItem(player))
		{
			if (Main.myPlayer == player.whoAmI)
			{
				if (player.altFunctionUse != 2)
				{
					Item.useAnimation = 5;
					Item.useTime = 5;
					Item.noMelee = true;
					Item.noUseGraphic = true;
					Item.autoReuse = false;
					Item.hammer = 0;
					var proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<DragonScaleHammerProj>(), player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
					proj.netUpdate2 = true;
				}
			}
			return false;
		}
		return base.CanUseItem(player);
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		return false;
	}
}
