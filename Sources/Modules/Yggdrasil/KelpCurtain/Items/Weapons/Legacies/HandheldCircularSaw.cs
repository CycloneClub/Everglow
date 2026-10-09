using Everglow.Yggdrasil.KelpCurtain.Projectiles.Legacies;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons.Legacies;

public class HandheldCircularSaw : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 46;
		Item.height = 54;
		Item.useAnimation = 5;
		Item.useTime = 5;
		Item.shootSpeed = 5f;
		Item.knockBack = 1.5f;
		Item.damage = 24;
		Item.rare = ItemRarityID.Green;

		Item.DamageType = DamageClass.Melee;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.value = Item.sellPrice(gold: 1);
	}

	public override bool CanUseItem(Player player)
	{
		if (player.ownedProjectileCounts[ModContent.ProjectileType<HandheldCircularSaw_proj>()] == 0)
		{
			if (Main.myPlayer == player.whoAmI)
			{
				var p0 = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<HandheldCircularSaw_proj>(), player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
				Vector2 mouseToPlayer = Main.MouseWorld - player.MountedCenter;
				mouseToPlayer = Vector2.Normalize(mouseToPlayer);
				p0.rotation = mouseToPlayer.ToRotation() + MathHelper.PiOver4;
			}
			return false;
		}
		return base.CanUseItem(player);
	}
}
