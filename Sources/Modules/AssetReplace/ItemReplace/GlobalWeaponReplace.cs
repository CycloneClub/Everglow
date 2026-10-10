using Everglow.AssetReplace.ItemReplace.Item_4923_PiercingStarlight;
using Everglow.Commons.FeatureFlags;
using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Mono.Cecil;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.AssetReplace.ItemReplace;

public class GlobalWeaponReplace : GlobalItem
{
	public override void SetDefaults(Item entity)
	{
		if (entity.type == ItemID.PiercingStarlight)
		{
			entity.shoot = ModContent.ProjectileType<PiercingStarlight_new>();
		}
		base.SetDefaults(entity);
	}

	public override bool InstancePerEntity => true;

	/// <summary>
	/// Remaining cooling time.
	/// </summary>
	public int CurrentPowerfulStabCD_Starlight;

	public override void UpdateInventory(Item entity, Player player)
	{
		if (entity.type == ItemID.PiercingStarlight)
		{
			if (CurrentPowerfulStabCD_Starlight > 0)
			{
				CurrentPowerfulStabCD_Starlight--;
			}
			else
			{
				CurrentPowerfulStabCD_Starlight = 0;
			}
		}
		base.UpdateInventory(entity, player);
	}

	public override bool AltFunctionUse(Item entity, Player player)
	{
		if (entity.type == ItemID.PiercingStarlight)
		{
			if (CurrentPowerfulStabCD_Starlight > 0)
			{
				return false;
			}

			foreach (Projectile proj in Main.projectile)
			{
				if (proj.owner == player.whoAmI && proj.timeLeft > 1 && proj.type == ModContent.ProjectileType<PiercingStarlight_new_Stab>())
				{
					return false;
				}
			}
			return true;
		}
		return base.AltFunctionUse(entity, player);
	}

	public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (item.type == ItemID.PiercingStarlight)
		{
			if (player.altFunctionUse == 2)
			{
				CurrentPowerfulStabCD_Starlight = 30;
				Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<PiercingStarlight_new_Stab>(), (int)(damage * 4.0f), knockback * 2, player.whoAmI, 0f, 0f);
				player.itemTime = item.useAnimation / 4;
				player.itemAnimation = item.useAnimation / 4;

				if (!EverglowConfig.DebugMode) // TODO: Test to see if velocity changes after stab attack is good. If not, delete the entire statement
				{
					if (player.direction == 1)
					{
						player.velocity.X += 1f;
					}
					else
					{
						player.velocity.X -= 1f;
					}
				}
				return false;
			}
			return true;
		}
		return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
	}

	public override bool CanUseItem(Item item, Player player)
	{
		if (item.type == ItemID.PiercingStarlight)
		{
			foreach (Projectile proj in Main.projectile)
			{
				if (proj.owner == player.whoAmI && proj.timeLeft > 1 && proj.type == ModContent.ProjectileType<PiercingStarlight_new_Stab>())
				{
					return false;
				}
			}
			return true;
		}
		return base.CanUseItem(item, player);
	}
}
