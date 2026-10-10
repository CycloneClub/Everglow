using Everglow.Yggdrasil.KelpCurtain.Projectiles.Legacies;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons.Legacies;

public class AncientSyringe : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 58;
		Item.height = 58;
		Item.useAnimation = 27;
		Item.useTime = 27;

		Item.damage = 45;
		Item.rare = ItemRarityID.Green;

		Item.DamageType = DamageClass.Magic;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.shoot = ModContent.ProjectileType<AncientSyringe_proj>();
		Item.shootSpeed = 5f;
		Item.value = Item.sellPrice(gold: 1);
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.ownedProjectileCounts[type] == 0)
		{
			if (Main.myPlayer == player.whoAmI)
			{
				var p0 = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, type, player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
				Vector2 mouseToPlayer = Main.MouseWorld - player.MountedCenter;
				mouseToPlayer = Vector2.Normalize(mouseToPlayer);
				p0.rotation = mouseToPlayer.ToRotation() + MathHelper.PiOver4;
			}
		}
		return false;
	}
}
