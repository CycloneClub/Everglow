namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

public class TendonGreatbow_Arrow : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	// Approved artwork is missing from the repository despite the Feishu artwork checkbox;
	// reuse the existing shared fallback texture rather than create placeholder art.
	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;

		Projectile.DamageType = DamageClass.Ranged;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
		Projectile.tileCollide = true;
		Projectile.arrow = true;
	}

	public override void AI()
	{
		// Mirror the CyatheaArrow_proj arrow flight so the projectile behaves like the
		// vanilla wooden arrow it replaces.
		Projectile.velocity.Y += 0.1f;
		if (Projectile.velocity.Y > 16f)
		{
			Projectile.velocity.Y = 16f;
		}
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		// 对Boss单位额外造成10%伤害
		if (target.boss)
		{
			modifiers.FinalDamage *= 1.1f;
		}
	}

	// The fallback texture is a single frame, so no frame-sliced PreDraw is added here.
}
