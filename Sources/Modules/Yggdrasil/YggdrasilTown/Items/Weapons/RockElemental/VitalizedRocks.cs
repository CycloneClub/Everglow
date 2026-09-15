using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.RockElemental;

public class VitalizedRocks : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicWeapons;

	public override void SetDefaults()
	{
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.width = 52;
		Item.height = 52;
		Item.useAnimation = 15;
		Item.useTime = 15;
		Item.knockBack = 3f;
		Item.damage = 25;
		Item.rare = ItemRarityID.Green;
		Item.UseSound = SoundID.Item1;
		Item.value = 12000;
		Item.autoReuse = false;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 9;
		Item.noUseGraphic = true;
		Item.autoReuse = true;
		Item.noMelee = true;

		Item.shoot = ModContent.ProjectileType<Projectiles.Magic.AmberMagicOrb>();
		Item.shootSpeed = 12;
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
					if (proj.type == ModContent.ProjectileType<VitalizedRocksProj>())
					{
						VitalizedRocksProj cvts = proj.ModProjectile as VitalizedRocksProj;
						if (cvts != null)
						{
							hasTarget = true;
						}
					}
				}
			}
		}
		if (!hasTarget)
		{
			Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.zeroVector, ModContent.ProjectileType<VitalizedRocksProj>(), Item.damage, Item.knockBack, player.whoAmI);
		}
		base.HoldItem(player);
	}
}
