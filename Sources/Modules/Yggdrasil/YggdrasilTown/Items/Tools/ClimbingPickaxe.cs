using ReLogic.Content;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools;

public class ClimbingPickaxe : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.DiamondHook);
		Item.shootSpeed = 18f;
		Item.shoot = ModContent.ProjectileType<ClimbingPickaxeProjectile>();

		// Item.useStyle = ItemUseStyleID.None;
		// Item.useTime = Item.useAnimation = 0;
		// Item.noUseGraphic = true;
	}
}

public class ClimbingPickaxeProjectile : ModProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MiscsProjectiles;

	public override string Texture => ModAsset.ClimbingPickaxe_Hook_Mod;

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.GemHookDiamond);
	}

	public override bool? CanUseGrapple(Player player)
	{
		int hooksOut = 0;
		foreach (var projectile in Main.ActiveProjectiles)
		{
			if (projectile.owner == Main.myPlayer && projectile.type == Projectile.type)
			{
				hooksOut++;
			}
		}

		return hooksOut <= 1;
	}

	public override float GrappleRange()
	{
		return 500f;
	}

	/// <summary>
	/// The amount of hooks that can be shot out
	/// </summary>
	/// <param name="player"></param>
	/// <param name="numHooks"></param>
	public override void NumGrappleHooks(Player player, ref int numHooks)
	{
		numHooks = 1;
	}

	/// <summary>
	/// How fast the grapple returns to you after meeting its max shoot distance.
	/// Default is 11, Lunar is 24
	/// </summary>
	/// <param name="player"></param>
	/// <param name="speed"></param>
	public override void GrappleRetreatSpeed(Player player, ref float speed)
	{
		speed = 18f;
	}

	/// <summary>
	/// Define how fast you get pulled to the grappling hook projectile's landing position
	/// </summary>
	/// <param name="player"></param>
	/// <param name="speed"></param>
	public override void GrapplePullSpeed(Player player, ref float speed)
	{
		speed = 10f;
	}

	/// <summary>
	/// Adjusts the position that the player will be pulled towards. This will make them hang 50 pixels away from the tile being grappled.
	/// </summary>
	/// <param name="player"></param>
	/// <param name="grappleX"></param>
	/// <param name="grappleY"></param>
	public override void GrappleTargetPoint(Player player, ref float grappleX, ref float grappleY)
	{
		Vector2 dirToPlayer = Projectile.DirectionTo(player.Center);
		float hangDist = 50f;
		grappleX += dirToPlayer.X * hangDist;
		grappleY += dirToPlayer.Y * hangDist;
	}

	/// <summary>
	/// Customize what tiles this hook can latch onto
	/// </summary>
	/// <param name="player"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public override bool? GrappleCanLatchOnTo(Player player, int x, int y)
	{
		Tile tile = Main.tile[x, y];
		if (TileID.Sets.IsATreeTrunk[tile.TileType] || tile.TileType == TileID.PalmTree)
		{
			return true;
		}

		return null;
	}

	public override bool PreDrawExtras(Player player)
	{
		Texture2D chainTexture = ModAsset.AmberFlowerHook_Chain.Value;
		Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
		Vector2 center = Projectile.Center;
		Vector2 directionToPlayer = playerCenter - Projectile.Center;
		float chainRotation = directionToPlayer.ToRotation();
		float distanceToPlayer = directionToPlayer.Length();

		while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer))
		{
			directionToPlayer /= distanceToPlayer; // get unit vector
			directionToPlayer *= chainTexture.Height; // multiply by chain link length

			center += directionToPlayer; // update draw position
			directionToPlayer = playerCenter - center; // update distance
			distanceToPlayer = directionToPlayer.Length();

			Color drawColor = new Color(0.8f, 0.4f, 0.5f).MultiplyRGB(Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16)));

			// Draw chain
			Main.EntitySpriteDraw(chainTexture, center - Main.screenPosition,
				chainTexture.Bounds, drawColor, chainRotation,
				chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		}

		return false;
	}
}
