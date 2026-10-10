using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

public class TwilightEucalyptusBoomerangKnift_Projectile : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	private const int Lifetime = 400;
	private const int ReboundTime = 30;
	private const int KillDistance = 3000;

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
	}

	public override void SetDefaults()
	{
		Projectile.width = 50;
		Projectile.height = 40;
		Projectile.ignoreWater = true;
		Projectile.friendly = true;

		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 6;
		Projectile.penetrate = -1;
		Projectile.timeLeft = Lifetime;
		Projectile.DamageType = DamageClass.Melee;
	}

	public override string Texture => ModAsset.TwilightEucalyptusBoomerangKnift_Mod;

	public override void AI()
	{
		BoomerangAI();
	}

	private void BoomerangAI()
	{
		// Boomerang rotation
		Projectile.rotation += 0.4f * Projectile.direction;

		// Boomerang sound
		if (Projectile.soundDelay == 0)
		{
			Projectile.soundDelay = 8;
			SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
		}

		// Returns after some number of frames in the air
		if (Projectile.timeLeft < Lifetime - ReboundTime)
		{
			Projectile.ai[0] = 1f;
		}

		if (Projectile.ai[0] == 1f)
		{
			Player player = Main.player[Projectile.owner];
			float returnSpeed = 14f;
			float acceleration = 0.6f;
			Vector2 playerVec = player.Center - Projectile.Center;
			float dist = playerVec.Length();

			// Delete the projectile if it's excessively far away.
			if (dist > KillDistance)
			{
				Projectile.Kill();
			}

			playerVec.Normalize();
			playerVec *= returnSpeed;

			// Home back in on the player.
			if (Projectile.velocity.X < playerVec.X)
			{
				Projectile.velocity.X += acceleration;
				if (Projectile.velocity.X < 0f && playerVec.X > 0f)
				{
					Projectile.velocity.X += acceleration;
				}
			}
			else if (Projectile.velocity.X > playerVec.X)
			{
				Projectile.velocity.X -= acceleration;
				if (Projectile.velocity.X > 0f && playerVec.X < 0f)
				{
					Projectile.velocity.X -= acceleration;
				}
			}
			if (Projectile.velocity.Y < playerVec.Y)
			{
				Projectile.velocity.Y += acceleration;
				if (Projectile.velocity.Y < 0f && playerVec.Y > 0f)
				{
					Projectile.velocity.Y += acceleration;
				}
			}
			else if (Projectile.velocity.Y > playerVec.Y)
			{
				Projectile.velocity.Y -= acceleration;
				if (Projectile.velocity.Y > 0f && playerVec.Y < 0f)
				{
					Projectile.velocity.Y -= acceleration;
				}
			}

			// Delete the projectile if it touches its owner.
			if (Main.myPlayer == Projectile.owner)
			{
				if (Projectile.Hitbox.Intersects(player.Hitbox))
				{
					Projectile.Kill();
				}
			}
		}
	}

	// Bounce on tiles.
	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
		Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

		if (Projectile.velocity.X != oldVelocity.X)
		{
			Projectile.velocity.X = -oldVelocity.X;
		}
		if (Projectile.velocity.Y != oldVelocity.Y)
		{
			Projectile.velocity.Y = -oldVelocity.Y;
		}
		Projectile.ai[0] = 1f;
		return false;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}
}
