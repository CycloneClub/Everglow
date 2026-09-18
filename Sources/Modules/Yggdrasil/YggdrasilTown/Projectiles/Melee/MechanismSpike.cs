using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

/// <summary>
/// 机关长戟概率掉出的刺球弹幕
/// </summary>
public class MechanismSpike : ModProjectile
{
	private int collideCount = 8;
	private float _rotationSpeed;
	private bool _initialized;

	private const int TrailLength = 15;
	private Vector2[] _trailPositions = new Vector2[TrailLength];

	private int _soundTimer;

	public override void SetDefaults()
	{
		Projectile.width = 28;
		Projectile.height = 28;
		Projectile.friendly = true;
		Projectile.timeLeft = 300;
		Projectile.tileCollide = true;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.aiStyle = 0;

		ProjectileID.Sets.TrailCacheLength[Type] = TrailLength;
		ProjectileID.Sets.TrailingMode[Type] = 0;

		Projectile.penetrate = 2;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
	}

	public override void AI()
	{
		if (!_initialized)
		{
			_rotationSpeed = Main.rand.NextBool() ? 0.25f : -0.25f;
			Projectile.velocity *= 25f;
			_initialized = true;
		}
		else
		{
			Projectile.velocity.Y += 0.4f;
			if (Projectile.velocity.Y > 16f)
			{
				Projectile.velocity.Y = 16f;
			}
		}

		Projectile.rotation += _rotationSpeed;

		Lighting.AddLight(Projectile.Center, 0.6f, 0.6f, 0.65f);

		if (++_soundTimer >= 12)
		{
			_soundTimer = 0;
			SoundEngine.PlaySound(SoundID.Item24 with { Volume = 0.4f, Pitch = -0.2f }, Projectile.Center);
		}

		if (Main.rand.NextBool(3)) // 控制密度，3 帧一次
		{
			Vector2 tailDir = -Projectile.velocity.SafeNormalize(Vector2.UnitY);
			Vector2 dustPos = Projectile.Center + tailDir * 10f;
			Vector2 dustVel = tailDir * Main.rand.NextFloat(1f, 2.5f);

			Dust d = Dust.NewDustPerfect(dustPos, DustID.Torch, dustVel, 150,
				new Color(Main.rand.Next(230, 255), Main.rand.Next(150, 190), 40),
				Main.rand.NextFloat(0.5f, 0.7f));
			d.noGravity = true;
			d.fadeIn = Main.rand.NextFloat(1.2f, 1.6f);
		}

		for (int i = TrailLength - 1; i > 0; i--)
		{
			_trailPositions[i] = _trailPositions[i - 1];
		}

		_trailPositions[0] = Projectile.Center;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (--collideCount == 0)
		{
			return true;
		}
		SoundEngine.PlaySound(SoundID.Item10 with { Volume = 0.6f }, Projectile.Center);

		if (Projectile.velocity.X != oldVelocity.X)
		{
			Projectile.velocity.X = -oldVelocity.X;
		}

		if (Projectile.velocity.Y != oldVelocity.Y)
		{
			Projectile.velocity.Y = -oldVelocity.Y;
		}

		Projectile.velocity *= 0.7f;

		for (int i = 0; i < 3; i++)
		{
			Vector2 vel = Main.rand.NextVector2Circular(2f, 2f);
			var dust = Dust.NewDustPerfect(Projectile.Center, DustID.Silver, vel);
			dust.noGravity = true;
			dust.scale = Main.rand.NextFloat(0.4f, 0.8f);
		}

		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Projectile.localNPCImmunity[target.whoAmI] = 30;
		target.immune[Projectile.owner] = 0;

		Projectile.penetrate--;
		if (Projectile.penetrate <= 0)
		{
			Projectile.Kill();
		}
	}

	public override bool? CanHitNPC(NPC target)
	{
		return Projectile.penetrate > 0;
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.6f, Pitch = -0.1f }, Projectile.Center);

		for (int i = 0; i < 10; i++)
		{
			Vector2 vel = Main.rand.NextVector2Circular(2f, 2f);
			var dust = Dust.NewDustPerfect(Projectile.Center, DustID.Silver, vel);
			dust.noGravity = true;
			dust.scale = Main.rand.NextFloat(0.6f, 1.0f);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
		Vector2 origin = texture.Size() / 2f;

		for (int i = 1; i < _trailPositions.Length; i++)
		{
			if (_trailPositions[i] == Vector2.Zero)
			{
				continue;
			}

			float t = i / (float)_trailPositions.Length;
			Color trailColor = Color.Lerp(Color.Silver, Color.Transparent, t) * 0.6f;
			float scale = Projectile.scale * (1f - 0.6f * t);

			Main.spriteBatch.Draw(
				texture,
				_trailPositions[i] - Main.screenPosition,
				null,
				trailColor,
				Projectile.rotation,
				origin,
				scale,
				SpriteEffects.None,
				0f);
		}

		Main.spriteBatch.Draw(
			texture,
			Projectile.Center - Main.screenPosition,
			null,
			Color.White,
			Projectile.rotation,
			origin,
			Projectile.scale * 1.05f,
			SpriteEffects.None,
			0f);

		return false;
	}
}
