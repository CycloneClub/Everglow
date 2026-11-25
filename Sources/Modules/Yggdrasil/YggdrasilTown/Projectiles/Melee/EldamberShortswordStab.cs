using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.Eldamber;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

public class EldamberShortswordStab : ModProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MeleeProjectiles;

	public override LocalizedText DisplayName => ModContent.GetInstance<EldamberShortsword>().DisplayName;

	public override string Texture => ModAsset.EldamberShortsword_Mod;

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.GoldShortswordStab);
		AIType = ProjectileID.GoldShortswordStab;

		// Center the 30 × 30 item sprite on the stab hitbox.
		DrawOffsetX = (Projectile.width - 30) / 2;
		DrawOriginOffsetY = (Projectile.height - 30) / 2;
	}

	public override void PostAI()
	{
		// Vanilla applies this angle to diagonal shortsword sprites when drawing them.
		Projectile.rotation -= MathHelper.PiOver4 * Projectile.spriteDirection;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		// Vanilla checks the body of the stab as well as the line to its tip.
		float collisionPoint = 0f;
		return projHitbox.Intersects(targetHitbox) || Collision.CheckAABBvLineCollision(
			targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
			Projectile.Center + Projectile.velocity * 6f, 10f * Projectile.scale, ref collisionPoint);
	}

	public override void CutTiles()
	{
		Vector2 tip = Projectile.Center + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 10f;
		Utils.PlotTileLine(Projectile.Center, tip, 10f * Projectile.scale, DelegateMethods.CutTiles);
	}
}
