using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.RockElemental;

public class HyperockSpear : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Swing;
		Item.width = 64;
		Item.height = 64;
		Item.useAnimation = 18;
		Item.useTime = 18;
		Item.knockBack = 8f;
		Item.damage = 22;
		Item.crit = 6;
		Item.rare = ItemRarityID.Green;
		Item.UseSound = SoundID.Item1;
		Item.value = 5500;
		Item.ArmorPenetration = 5;
		Item.autoReuse = false;
		Item.DamageType = DamageClass.Melee;
		Item.channel = true;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.shoot = ModContent.ProjectileType<HyperockSpearProj>();
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		return false;
	}

	public override void HoldItem(Player player)
	{
		player.ListenMouseWorld();

		bool hasTarget = player.itemAnimation > 0;
		foreach (Projectile proj in Main.projectile)
		{
			if (proj.active)
			{
				if (proj.owner == player.whoAmI)
				{
					if (proj.type == ModContent.ProjectileType<HyperockSpearProj>())
					{
						HyperockSpearProj cvts = proj.ModProjectile as HyperockSpearProj;
						if (cvts != null)
						{
							if (!cvts.Shot && !cvts.CollideOnNPC && !cvts.CollideOnTile)
							{
								hasTarget = true;
							}
						}
					}
				}
			}
		}
		if (!hasTarget && !player.controlUseItem)
		{
			Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.zeroVector, ModContent.ProjectileType<HyperockSpearProj>(), Item.damage, Item.knockBack, player.whoAmI);
		}
		base.HoldItem(player);
	}
}
