using Everglow.Yggdrasil.YggdrasilTown.VFXs;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

internal class FeatheredStaff : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	private const int MaxCollisionCount = 2;

	private const int P1Duration = 90;
	private const float P1FOVDistance = 500;
	private const float P1FOVAngle = MathF.PI / 2.0f;
	private const float P1RotationSpeed = 0.02f;

	private const float P2VelocityLimitY = 10f;
	private const float P2GravityCoef = 0.2f;

	private int TargetWhoAmI { get; set; } = -1;

	private bool HasNotBursted { get; set; } = true;

	private int CollisionCounter
	{
		get => (int)Projectile.ai[0];

		set => Projectile.ai[0] = value;
	}

	private int LifeTimer
	{
		get => (int)Projectile.ai[1];

		set => Projectile.ai[1] = value;
	}

	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 8;
	}

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.aiStyle = 0;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.penetrate = 3;
		Projectile.timeLeft = 300;
		Projectile.tileCollide = true;

		CollisionCounter = 0;
		LifeTimer = 0;
	}

	public override void AI()
	{
		if (Projectile.velocity.Y > 10f)
		{
			Projectile.velocity.Y = 10f;
		}

		if (LifeTimer < P1Duration)
		{
			P1AI();
		}
		else
		{
			P2AI();
		}

		LifeTimer++;
	}

	// Phase 1: No gravity, track enemy within the field of view (a sector)
	// --------------------------------------------------------------------
	private void P1AI()
	{
		CheckTargetActive();

		if (TargetWhoAmI == -1)
		{
			FindEnemy();
			return;
		}
		else // track enemy
		{
			NPC npc = Main.npc[TargetWhoAmI];

			Vector2 directionToTarget = npc.Center - Projectile.Center;
			directionToTarget.Normalize();

			var angleToTarget = MathHelper.WrapAngle(
				directionToTarget.ToRotation() -
				Projectile.velocity.ToRotation());

			if (Math.Abs(angleToTarget) > P1RotationSpeed)
			{
				// Rotate by the rotation speed
				if (angleToTarget > 0)
				{
					Projectile.velocity = Projectile.velocity.RotatedBy(P1RotationSpeed);
				}
				else
				{
					Projectile.velocity = Projectile.velocity.RotatedBy(-P1RotationSpeed);
				}
			}
			else
			{
				// Rotate directly to the target direction
				Projectile.velocity.RotatedBy(angleToTarget);
			}
		}
	}

	public void GenerateParticles(int duplicateTimes = 1)
	{
		for (int i = 0; i < duplicateTimes; i++)
		{
			Vector2 newVelocity = Projectile.velocity.NormalizeSafe() * 1.5f;
			var somg = new DarkGlimmeringParticleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + Projectile.velocity * i / duplicateTimes,
				MaxTime = Main.rand.Next(27, 66),
				Scale = 1,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 7, (float)(Main.time + i / (float)duplicateTimes) * 1.8f },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public void GenerateParticlesExposion(int duplicateTimes = 1)
	{
		for (int i = 0; i < duplicateTimes; i++)
		{
			float speed = MathF.Pow(Main.rand.NextFloat(1f), 0.15f) * 5f;
			Vector2 newVelocity = new Vector2(0, speed).RotatedByRandom(MathHelper.TwoPi);
			var somg = new DarkGlimmeringParticleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(27, 36),
				Scale = 1,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 5, (float)Main.time * 1.8f - speed * 2.4f },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	// Phase 2: Gravity gradually increase to max value
	// ------------------------------------------------
	private void P2AI()
	{
		if (HasNotBursted)
		{
			GenerateParticlesExposion(50);

			Gore.NewGore(Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Everglow/FeatheredStaff_gore0").Type, Projectile.scale);
			Gore.NewGore(Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Everglow/FeatheredStaff_gore1").Type, Projectile.scale);
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<FeatheredStaff_break>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack * 0.2f, Projectile.owner, 0.57f);
			HasNotBursted = false;
		}
		GenerateParticles(5);
		Projectile.velocity *= 0.99f;

		// Gravity
		Projectile.velocity.Y =
			Projectile.velocity.Y + P2GravityCoef < P2VelocityLimitY ?
			Projectile.velocity.Y + P2GravityCoef :
			P2VelocityLimitY;
	}

	private void FindEnemy()
	{
		float minDistance = P1FOVDistance + 1;

		foreach (NPC npc in Main.npc)
		{
			if (npc.active)
			{
				// Outside the field of view
				if (!CheckTargetWithinFOV(npc))
				{
					continue;
				}

				if (npc.dontTakeDamage || npc.friendly || !npc.CanBeChasedBy() || !Collision.CanHit(Projectile, npc))
				{
					continue;
				}

				if (Projectile.Center.Distance(npc.Center) >= minDistance)
				{
					continue;
				}

				TargetWhoAmI = npc.whoAmI;
				minDistance = Projectile.Center.Distance(npc.Center);
			}
		}
	}

	private void CheckTargetActive()
	{
		if (TargetWhoAmI == -1)
		{
			return;
		}

		NPC npc = Main.npc[TargetWhoAmI];

		if (!npc.active || npc.dontTakeDamage)
		{
			TargetWhoAmI = -1;
			return;
		}

		if (!CheckTargetWithinFOV(npc))
		{
			TargetWhoAmI = -1;
			return;
		}
	}

	private bool CheckTargetWithinFOV(NPC npc)
	{
		Vector2 directionToTarget = npc.Center - Projectile.Center;

		Vector2 projectileDirection = Projectile.velocity;

		float angleToTarget = MathHelper.WrapAngle(projectileDirection.ToRotation() - directionToTarget.ToRotation());

		bool angleCheck = MathF.Abs(angleToTarget) <= P1FOVAngle / 2;

		bool distanceCheck = Projectile.Center.Distance(npc.Center) <= P1FOVDistance;

		return angleCheck && distanceCheck;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		CollisionCounter++;
		if (LifeTimer >= P1Duration)
		{
			Projectile.Kill();
			GenerateParticlesExposion(20);
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<FeatheredStaff_break>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack * 0.15f, Projectile.owner, 0.8f);
			return true;
		}
		if (CollisionCounter >= MaxCollisionCount)
		{
			Projectile.Kill();
			GenerateParticlesExposion(20);
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<FeatheredStaff_break>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack * 0.15f, Projectile.owner, 0.35f);
			return true;
		}
		GenerateParticlesExposion(13);
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<FeatheredStaff_break>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack * 0.1f, Projectile.owner, 0.27f);
		if (Projectile.velocity.X != oldVelocity.X)
		{
			Projectile.velocity.X = -oldVelocity.X * 0.8f;
		}
		if (Projectile.velocity.Y != oldVelocity.Y)
		{
			Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
		}
		return false;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if (LifeTimer < P1Duration)
		{
			Texture2D texture = ModAsset.Magic_FeatheredStaff.Value;

			Color drawColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());

			int frameHeight = texture.Height / Main.projFrames[Projectile.type];
			int frameY = frameHeight * Projectile.frame;
			var sourceRect = new Rectangle(0, frameY, texture.Width, frameHeight);

			var rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
			SpriteEffects effects = SpriteEffects.None;

			// If the projectile is facing left
			if (Projectile.velocity.X < 0 &&
				Projectile.oldVelocity.X < 0)
			{
				effects = SpriteEffects.FlipHorizontally;
				rotation = rotation - MathF.PI;
			}

			Main.spriteBatch.Draw(
				texture,
				Projectile.Center - Main.screenPosition,
				sourceRect,
				drawColor,
				rotation,
				origin: new Vector2(texture.Width, texture.Width) / 2.0f,
				scale: 1,
				effects: effects,
				0);

			Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
		}
		else
		{
			Texture2D texture = ModContent.Request<Texture2D>(BrokenTexture(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

			Color color = Lighting.GetColor(Projectile.Center.ToTileCoordinates());

			var rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);

			SpriteEffects effects = SpriteEffects.None;

			// If the projectile is facing left
			if (Projectile.velocity.X < 0 &&
				Projectile.oldVelocity.X < 0)
			{
				effects = SpriteEffects.FlipHorizontally;
				rotation = rotation - MathF.PI;
			}

			Main.spriteBatch.Draw(
				texture,
				Projectile.Center - Main.screenPosition,
				null,
				new Color(1f, 1f, 0.4f, 0),
				rotation,
				origin: new Vector2(texture.Width, texture.Width) / 2.0f,
				scale: 1,
				effects: effects,
				0);
			Texture2D light = Commons.ModAsset.StarSlash.Value;
			var lightHalo = Color.Lerp(new Color(1f, 0.8f, 0.5f, 0), new Color(0.7f, 0.2f, 0.1f, 0), (float)(Math.Sin(Main.time * 0.3f + Projectile.whoAmI) + 1) * 0.5f);
			Main.spriteBatch.Draw(
				light,
				Projectile.Center - Main.screenPosition,
				null,
				lightHalo,
				rotation + MathHelper.PiOver2,
				light.Size() * 0.5f,
				new Vector2(0.5f, 0.7f),
				effects: effects,
				0);
			Main.spriteBatch.Draw(
				light,
				Projectile.Center - Main.screenPosition,
				null,
				lightHalo,
				0.75f,
				light.Size() * 0.5f,
				new Vector2(0.2f, 0.3f),
				effects: effects,
				0);
			Main.spriteBatch.Draw(
				light,
				Projectile.Center - Main.screenPosition,
				null,
				lightHalo,
				-0.75f,
				light.Size() * 0.5f,
				new Vector2(0.2f, 0.3f),
				effects: effects,
				0);
		}

		return false;
	}

	private static string BrokenTexture() => ModAsset.FeatheredStaff_broken_Mod;

	public override void OnKill(int timeLeft)
	{
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		GenerateParticlesExposion(20);
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<FeatheredStaff_break>(), (int)(Projectile.damage * 0.4f), Projectile.knockBack * 0.15f, Projectile.owner, 0.35f);
	}
}
