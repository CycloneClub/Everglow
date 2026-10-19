using Everglow.Commons.Utilities;
using Terraria.DataStructures;
using static Terraria.GameContent.Prefixes.PrefixLegacy;

namespace Everglow.Commons.Templates.Weapons.Clubs;

/// <summary>
/// A template for club-type weapons.
/// </summary>
public abstract class ClubItem : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeWeapons;

	/// <summary>
	/// Type of projectile used for normal attack.
	/// </summary>
	public int ProjType { get; protected set; }

	/// <summary>
	/// Type of projectile used for smash down attack.
	/// </summary>
	public int ProjSmashType { get; protected set; }

	/// <summary>
	/// Damage multiplier applied when creating a smash down attack.
	/// </summary>
	public float SmashDamageMultiplier { get; protected set; } = 1.62f;

	public override void SetStaticDefaults()
	{
		ItemSets.SwordsHammersAxesPicks[Type] = true;
	}

	public sealed override void SetDefaults()
	{
		Item.damage = 5;
		Item.width = 48;
		Item.height = 48;
		Item.useTime = 4;
		Item.value = 50;
		Item.useTime = 8;
		Item.useAnimation = 8;

		Item.autoReuse = true;
		Item.noMelee = true;
		Item.noUseGraphic = true;

		Item.shootSpeed = 1f;
		Item.knockBack = 16f;

		Item.DamageType = DamageClass.Melee;
		Item.useStyle = ItemUseStyleID.Thrust;
		Item.rare = ItemRarityID.White;

		SetCustomDefaults();
		Item.shoot = ProjType;
	}

	/// <summary>
	/// This is where you set all your item's stat, such as width, damage, knockback, etc.
	/// <br/>Also special properties of club weapon: <see cref="ProjType"/> and <see cref="ProjSmashType"/>.
	/// <para/>Called after <see cref="SetDefaults"/>. The native <see cref="SetDefaults"/> has been marked as <c>sealed</c> for better protection.
	/// </summary>
	public virtual void SetCustomDefaults()
	{
	}

	public override bool AllowPrefix(int pre) => true;

	public override bool MeleePrefix() => true;

	public override bool AltFunctionUse(Player player) => ProjSmashType > ProjectileID.None && CanSmashDown(player);

	private static bool CanSmashDown(Player player)
	{
		for (int h = 0; h < 7; h++)
		{
			Vector2 pos = player.Center + new Vector2(0, h * 16 * player.gravDir);
			Point bottomPos = pos.ToTileCoordinates();
			bottomPos.X = Math.Clamp(bottomPos.X, 20, Main.maxTilesX - 20);
			bottomPos.Y = Math.Clamp(bottomPos.Y, 20, Main.maxTilesY - 20);
			if (TileUtils.PlatformCollision(pos) || ((player.waterWalk || player.waterWalk2) && Main.tile[bottomPos].LiquidAmount > 0 && !player.wet))
			{
				return false;
			}
		}
		for (int h = 7; h < 120; h++)
		{
			Vector2 pos = player.Center + new Vector2(0, h * 16 * player.gravDir);
			Point bottomPos = pos.ToTileCoordinates();
			bottomPos.X = Math.Clamp(bottomPos.X, 20, Main.maxTilesX - 20);
			bottomPos.Y = Math.Clamp(bottomPos.Y, 20, Main.maxTilesY - 20);
			if (TileUtils.PlatformCollision(pos) || ((player.waterWalk || player.waterWalk2) && Main.tile[bottomPos].LiquidAmount > 0 && !player.wet))
			{
				return true;
			}
		}
		return false;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
	{
		if (player.altFunctionUse != 2)
		{
			// Left click: Hold and rotate.
			if (player.ownedProjectileCounts[type] < 1)
			{
				Projectile.NewProjectile(source, position + velocity * 2f, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
			}
		}
		else
		{
			// Right click: Smash down.
			if (player.ownedProjectileCounts[ProjSmashType] < 1 && Main.mouseLeftRelease)
			{
				player.mount.Dismount(player);
				var p = Projectile.NewProjectileDirect(source, position + velocity * 2f, Vector2.Zero, ProjSmashType, (int)(damage * SmashDamageMultiplier), knockback, player.whoAmI, 0f, 0f);
				p.scale = Item.scale;

				// Kill duplicate.
				if (player.ownedProjectileCounts[type] >= 1)
				{
					foreach (Projectile proj in Main.projectile)
					{
						if (proj.active && proj.owner == player.whoAmI && proj.type == type)
						{
							proj.Kill();
						}
					}
				}
			}
		}

		return false;
	}
}
