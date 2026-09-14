using Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee;
using Terraria.ID;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Weapons;

public class MeatLantern : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 116;
		Item.height = 132;
		Item.useAnimation = 25;
		Item.useTime = 25;
		Item.shootSpeed = 5f;
		Item.knockBack = 3f;
		Item.damage = 21;
		Item.rare = ItemRarityID.Green;

		Item.DamageType = DamageClass.Melee;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.value = Item.sellPrice(gold: 1);
	}
	public override bool CanUseItem(Player player)
	{
		if (base.CanUseItem(player))
		{
			if (Main.myPlayer == player.whoAmI)
			{
				if (player.altFunctionUse != 2)
					Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<MeatLantern_Proj>(), player.GetWeaponDamage(Item), Item.knockBack, player.whoAmI);
				else//右键
				{
				}
			}
			return false;
		}
		return base.CanUseItem(player);
	}
}
