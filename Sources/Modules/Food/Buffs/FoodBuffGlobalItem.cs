using Everglow.Commons.Utilities;
using Terraria.DataStructures;

namespace Everglow.Food.Buffs;

public class FoodBuffGlobalItem : GlobalItem
{
	public override bool InstancePerEntity => true;

	private int l = 0;

	public override void HorizontalWingSpeeds(Item item, Player player, ref float speed, ref float acceleration)
	{
		if (player.GetModPlayer<FoodBuffModPlayer>().RoastedBirdBuff)
		{
			// Vanilla has already selected the normal or hovering speed for the equipped wing.
			speed *= 1.15f;
		}
	}

	public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.GetModPlayer<FoodBuffModPlayer>().GreenStormBuff)
		{
			Projectile.NewProjectile(source, position + new Vector2(0, -24), l % 2 == 0 ? velocity : velocity / 10, l % 2 == 0 ? ProjectileID.CrystalLeafShot : ProjectileID.SporeCloud, damage, knockback, player.whoAmI, 0f, 0f);
			l++;
			return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
		}
		if (player.GetModPlayer<FoodBuffModPlayer>().StarfruitBuff)
		{
			if (ItemUtils.Ranged.Shotguns.Contains(item.type))
			{
				Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(5)) * (1f - Main.rand.NextFloat(0.1f));
				if (item.type == ItemID.Xenopopper)
				{
					int num = Projectile.NewProjectile(source, position, newVelocity / 4, ProjectileID.Xenopopper, damage, knockback, player.whoAmI);
					Main.projectile[num].localAI[0] = type;
					Main.projectile[num].localAI[1] = velocity.Length();
				}
				else
				{
					Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
				}

				return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
			}
		}
		if (player.GetModPlayer<FoodBuffModPlayer>().FriedEggBuff)
		{
			if (ItemUtils.Ranged.Consumables.Contains(item.type))
			{
				Projectile.NewProjectile(source, position, 1.5f * velocity, type, damage, knockback, player.whoAmI, 0);
				return false;
			}
		}
		return true;
	}

	public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
	{
		if (player.GetModPlayer<FoodBuffModPlayer>().NachosBuff && ItemUtils.Ranged.Launchers.Contains(item.type))
		{
			// +4% launcher damage
			damage += 0.04f;
		}
	}

	public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
	{
		if (player.GetModPlayer<FoodBuffModPlayer>().NachosBuff && ItemUtils.Ranged.Launchers.Contains(item.type))
		{
			// +4% launcher knockback
			knockback += 0.04f;
		}
	}

	public override void ModifyItemScale(Item item, Player player, ref float scale)
	{
		if (!item.IsAir && item.damage > 0 && !item.noMelee && item.pick == 0 && item.axe == 0 && item.hammer == 0)
		{
			if (player.GetModPlayer<FoodBuffModPlayer>().KiwiJuiceBuff)
			{
				// +150% weapon size
				scale += 1.5f;
			}

			if (player.GetModPlayer<FoodBuffModPlayer>().KiwiFruitBuff)
			{
				// +20% weapon size
				scale += 0.2f;
			}

			if (player.GetModPlayer<FoodBuffModPlayer>().KiwiIceCreamBuff)
			{
				// +10% weapon size
				scale += 0.1f;
			}
		}
	}
}
