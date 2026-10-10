using Everglow.Commons.Templates.Weapons;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.IstafelsEffects;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class IstafelsSunfireGrasp_FireBall : TrailingProjectile, IWarpProjectile_warpStyle2
{
	public override string Texture => Commons.ModAsset.Point_Mod;

	public const int ProjectileVelocity = 4;
	public const int SubProjectileVelocity = 12;
	private const float InitialScale = 0.5f;
	public const int TimeLeftMax = 600;

	public const int BuffDuration = 960;
	public const int ContactDamage = 25;
	public const int BuildUpMax = 200;

	/// <summary>
	/// If the projectile has been actived by <see cref="IstafelsSunfireGrasp"/>.
	/// </summary>
	public bool Actived { get; set; } = false;

	/// <summary>
	/// The build-up value of projectile, capped at <see cref="BuildUpMax"/>.
	/// </summary>
	public int BuildUp { get; set; } = 0;

	/// <summary>
	/// The build-up value of projectile, bewteen 0 and 1.
	/// </summary>
	public float BuildUpProgress => Math.Clamp(BuildUp / (float)BuildUpMax, 0f, 1f);

	/// <summary>
	/// Chase target.
	/// </summary>
	public int TargetWhoAmI
	{
		get => (int)Projectile.ai[0];
		set => Projectile.ai[0] = value;
	}

	public override void SetCustomDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;

		Projectile.DamageType = DamageClass.Magic;

		Projectile.friendly = false;
		Projectile.hostile = false;

		Projectile.timeLeft = TimeLeftMax;

		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.ignoreWater = true;

		Projectile.localNPCHitCooldown = 10;
		Projectile.usesLocalNPCImmunity = true;

		TrailLength = 15;
		TrailColor = new Color(0.7f, 0.1f, 0f, 0);
		TrailTexture = Commons.ModAsset.Trail_8.Value;
		TrailBackgroundDarkness = 0.5f;
		SelfLuminous = true;
		TargetWhoAmI = -1;
	}

	public override bool? CanCutTiles() => true;

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
		ShootScoria();
		return true;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		// Let the projectile deals true damage, minus base damage 1
		modifiers.FinalDamage.Flat += ContactDamage - 1;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<Charred>(), BuffDuration);
		ShootScoria();
	}

	/// <summary>
	/// Explode then shoot scoria. Only called when killed by tile colliding and npc colliding.
	/// </summary>
	private void ShootScoria()
	{
		// Shoot DestroyEntityEffect
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<IstafelsSunfireGrasp_Explosion>(), 200, Projectile.knockBack, Projectile.owner);

		// Shoot scoria
		var scoriaProjCount = Main.rand.Next(8, 10);
		for (int i = 0; i < scoriaProjCount; i++)
		{
			var velocity = Projectile.velocity * 0.6f + Vector2.UnitX.RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)) * 4f;
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, velocity, ModContent.ProjectileType<IstafelsSunfireGrasp_Scoria>(), 1, 1.1f, Projectile.owner);
		}
	}

	public override void AI()
	{
		base.AI();
	}

	public override void Behaviors()
	{
		Lighting.AddLight(Projectile.Center, 1.8f, 0.8f, 0f);
		if (Actived)
		{
			Projectile.rotation += Projectile.velocity.X * 0.01f;

			if (TargetWhoAmI < 0)
			{
				TargetWhoAmI = ProjectileUtils.FindTarget(Projectile.Center, 300);
				return;
			}

			// Generate dusts
			if (Main.rand.NextBool(6))
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi);
				var spark = new FireSpark_MetalStabDust
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = Main.rand.Next(1, 25),
					Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(10f, 27.0f)),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.13f, 0.13f) },
				};
				Ins.VFXManager.Add(spark);
			}

			// Chase target
			var target = Main.npc[TargetWhoAmI];
			if (target.active)
			{
				var distance = target.Center - Projectile.Center;
				Projectile.velocity = distance.NormalizeSafe() * ProjectileVelocity;
			}
			else
			{
				TargetWhoAmI = -1;
			}
		}
		else
		{
			var owner = Main.player[Projectile.owner];

			// Update life time
			if (owner.HasBuff<IstafelsSunfireGraspFireBallBuff>())
			{
				owner.AddBuff(ModContent.BuffType<IstafelsSunfireGraspFireBallBuff>(), 2);
				Projectile.timeLeft = TimeLeftMax;
			}
			else
			{
				Projectile.Kill();
				return;
			}

			// Chase player
			var destination = owner.Center + new Vector2(-30 * owner.direction, -50 * owner.gravDir);
			var movement = destination - Projectile.Center;
			float speedMax = movement.Length() * 0.1f + owner.velocity.Length();
			if (movement.Length() > 10f)
			{
				Projectile.velocity = movement.NormalizeSafe() * speedMax * 0.05f + Projectile.velocity * 0.95f;
			}

			if (Projectile.velocity.Length() > speedMax)
			{
				Projectile.velocity = Projectile.velocity.NormalizeSafe() * speedMax;
			}
			Projectile.velocity *= 0.9f;

			if (BuildUpProgress > 0.5f)
			{
				if (Main.rand.NextBool(26))
				{
					var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<IstafelsSunfireDrop_glimmer>());
					dust.customData = Projectile;
					dust.velocity = new Vector2(0, 1).RotatedByRandom(0.4f);
				}

				// Shoot projectile to target
			}
			if (Timer % 120 == 0)
			{
				var target = ProjectileUtils.FindTarget(Projectile.Center, 2000);
				if (target >= 0 && Collision.CanHit(Main.npc[target], Projectile))
				{
					var direction = (Main.npc[target].Center - Projectile.Center).NormalizeSafe();
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, direction * SubProjectileVelocity, ModContent.ProjectileType<IstafelsSunfireGrasp_Sub_FireBall>(), 50, 0.4f, Projectile.owner);
				}
			}
		}
	}

	/// <summary>
	/// Active projectile
	/// </summary>
	/// <param name="target"></param>
	public void Active(NPC target)
	{
		Actived = true;

		// Shoot projectile towards the target
		TargetWhoAmI = target.whoAmI;
		Projectile.netUpdate = true;
		Projectile.velocity = (target.Center - Projectile.Center).NormalizeSafe() * ProjectileVelocity;

		// Update misc infos
		Projectile.friendly = true;
		Projectile.tileCollide = true;
		Projectile.penetrate = 1;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var drawPos = Projectile.Center - Main.screenPosition;
		var drawScale = MathHelper.Lerp(InitialScale, 1f, BuildUpProgress);
		var drawSize = Projectile.width / 2 * drawScale;
		var blackBall = Commons.ModAsset.Ball_black.Value;
		Main.spriteBatch.Draw(blackBall, drawPos, null, Color.White, 0, blackBall.Size() * 0.5f, drawScale * 0.4f, SpriteEffects.None, 0);

		DrawTrail();

		var sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		var effect = ModAsset.IstafelsSunfireGrasp_FireBall.Value;
		effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.05f);
		effect.CurrentTechnique.Passes[0].Apply();

		var vertices = new List<Vertex2D>
		{
			{ drawPos + new Vector2(-1, -1).RotatedBy(Projectile.rotation) * drawSize, Color.White, new(0, 0, 0) },
			{ drawPos + new Vector2(1, -1).RotatedBy(Projectile.rotation) * drawSize, Color.White, new(1, 0, 0) },
			{ drawPos + new Vector2(-1, 1).RotatedBy(Projectile.rotation) * drawSize, Color.White, new(0, 1, 0) },
			{ drawPos + new Vector2(1, 1).RotatedBy(Projectile.rotation) * drawSize, Color.White, new(1, 1, 0) },
		};

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public override void DrawTrail()
	{
		base.DrawTrail();
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0) => base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor - timeValue * 0.2f;
		float y = 1;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.DD2_GoblinBomb, Projectile.Center);
		ShakerManager.AddShaker(Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.Pi), 60, 12f, 200, 0.9f, 0.8f, 600);
	}

	public new void DrawWarp(VFXBatch spriteBatch)
	{
		var drawPos = Projectile.Center - Main.screenPosition;
		var fade = 1f;
		var drawScale = MathHelper.Lerp(InitialScale, 1f, BuildUpProgress);
		var drawSize = MathF.Sqrt(Projectile.width / 2 * drawScale) * 0.27f;
		var timeValue = (float)Main.timeForVisualEffects * 0.003f;
		float resolutionCircle = 50;
		var bars = new List<Vertex2D>();
		var bars2 = new List<Vertex2D>();
		for (int i = 0; i <= resolutionCircle; i++)
		{
			var radiusInner = new Vector2(0, 10 * drawSize).RotatedBy(i / resolutionCircle * MathHelper.TwoPi);
			var radiusMiddle = new Vector2(0, 24 * drawSize).RotatedBy(i / resolutionCircle * MathHelper.TwoPi) - Projectile.velocity;
			var radiusOuter = new Vector2(0, 40 * drawSize).RotatedBy(i / resolutionCircle * MathHelper.TwoPi) - Projectile.velocity * 2;

			float rot = MathHelper.PiOver2;

			Vector2 dirInner = new Vector2(1, 0).RotatedBy(i / resolutionCircle * MathHelper.TwoPi + rot);
			Vector2 dirMiddle = (dirInner - Projectile.velocity.NormalizeSafe() * 0.15f).NormalizeSafe();
			Vector2 dirOuter = (dirMiddle - Projectile.velocity.NormalizeSafe() * 0.15f).NormalizeSafe();

			var colorInner = new Color(-dirInner.X * 0.5f + 0.5f, -dirInner.Y * 0.5f + 0.5f, 0, 0);
			var colorMiddle = new Color(-dirMiddle.X * 0.5f + 0.5f, -dirMiddle.Y * 0.5f + 0.5f, fade * 0.05f * Main.GameViewMatrix.Zoom.X, 0);
			var colorOuter = new Color(-dirOuter.X * 0.5f + 0.5f, -dirOuter.Y * 0.5f + 0.5f, 0, 0);

			bars.Add(drawPos + radiusMiddle, colorMiddle, new Vector3(timeValue, i / 50f, 1));
			bars.Add(drawPos + radiusInner, colorInner, new Vector3(timeValue + 0.15f, i / 50f, 1));

			bars2.Add(drawPos + radiusMiddle, colorMiddle, new Vector3(timeValue, i / 50f, 1));
			bars2.Add(drawPos + radiusOuter, colorOuter, new Vector3(timeValue - 0.15f, i / 50f, 1));
		}
		if (bars.Count > 2)
		{
			Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.graphicsDevice.RasterizerState = RasterizerState.CullNone;
			spriteBatch.Draw(Commons.ModAsset.Noise_melting_H.Value, bars, PrimitiveType.TriangleStrip);
		}
		if (bars2.Count > 2)
		{
			Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.graphicsDevice.RasterizerState = RasterizerState.CullNone;
			spriteBatch.Draw(Commons.ModAsset.Noise_melting_H.Value, bars2, PrimitiveType.TriangleStrip);
		}
	}
}
