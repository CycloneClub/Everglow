using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class NightfireStaff_Projectile : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public const int VelocityXMax = 16;
	public const int VelocityYMax = 4;
	public const float Acceleration = 0.05f;
	public const int TimeLeftMax = 600;

	/// <summary>
	/// The main direction of the pair of projectile shot by weapon
	/// </summary>
	public float MainDirection => Projectile.ai[0];

	/// <summary>
	/// Used to represent this projectile is which one of the pair of projectile shot by weapon
	/// </summary>
	public float Inversed => Projectile.ai[1];

	public override void SetStaticDefaults()
	{
		Main.projFrames[Type] = 4;
	}

	public override void SetDefaults()
	{
		Projectile.width = 8;
		Projectile.height = 8;
		Projectile.aiStyle = 0;
		Projectile.penetrate = 2;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.timeLeft = TimeLeftMax;
		Projectile.magic = true;
	}

	public override void AI()
	{
		// Update texture frame
		// ====================
		if (Main.time % 5 == 0)
		{
			Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
		}

		// Use percentage to represent the progress of projectile life cycle.
		// The progress will affect: lighting strength, velocity.
		// ======================================================
		var progress = (Projectile.timeLeft - TimeLeftMax) * 1f / TimeLeftMax;

		// Add green light to simulate the light of firefly.
		// Lighting Strength = Basic Strength + Progress Strength + Random Strength
		// ========================================================================
		var lightStrength = 0.7f + 0.3f * progress + 0.2f * Main.rand.NextFloat(-1, 1);
		Lighting.AddLight(Projectile.Center, 0, 0.52f * lightStrength, 0);

		// Calculate the velocity of projectile.
		// =====================================
		var velocity = Projectile.velocity.RotatedBy(-MainDirection);

		// Velocity on main direction.
		if (velocity.X + Acceleration > VelocityXMax)
		{
			velocity.X = VelocityXMax;
		}
		else
		{
			velocity.X = velocity.X + Acceleration;
		}

		// Velocity vertical to the main direction
		velocity.Y = Inversed * VelocityYMax * MathF.Cos(progress * 100 + MathHelper.Pi / 6);

		// Rotate the velocity to main direction
		Projectile.velocity = velocity.RotatedBy(MainDirection);

		// Generate dusts
		var dust = Dust.NewDustDirect(Projectile.Center, 2, 2, ModContent.DustType<NightFire>(), Projectile.velocity.X, Projectile.velocity.Y);
		dust.velocity = Projectile.velocity;
		dust.scale = Main.rand.NextFloat(1.1f, 1.7f);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		// Draw firefly framed texture
		Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
		Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
		float rotation = Projectile.velocity.ToRotation();
		SpriteEffects spriteEffect = SpriteEffects.FlipHorizontally;
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, rotation, frame.Size() / 2, Projectile.scale, spriteEffect, 0);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 10; i++)
		{
			Dust.NewDust(Projectile.Center, 2, 2, ModContent.DustType<NightFire>(), Projectile.velocity.X, Projectile.velocity.Y);
		}

		SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.Center);
	}
}
