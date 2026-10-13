using Everglow.Yggdrasil.YggdrasilTown.VFXs;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;

public class AncientPhantom_Bow_Arrow : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public override void SetDefaults()
	{
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.tileCollide = true;
		Projectile.aiStyle = -1;
		Projectile.maxPenetrate = 1;
		Projectile.timeLeft = 600;
	}

	public override void AI()
	{
		Projectile.velocity.Y += 0.1f;
		if (Projectile.velocity.Length() > 20f)
		{
			Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 20f;
		}
		Projectile.rotation = Projectile.velocity.ToRotationSafe() - MathHelper.PiOver2;
		if (Projectile.timeLeft % 2 == 0)
		{
			var dustVFX = new TwilightCrystalVFXDust
			{
				Velocity = Projectile.velocity * 0.1f,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = 60,
				Scale = Main.rand.NextFloat(3, 6),
				Rotation = Projectile.rotation + MathHelper.PiOver4,
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var tex = ModAsset.AncientPhantom_Bow_Arrow.Value;
		lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor * 0.9f, Projectile.rotation, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		return false;
	}
}
