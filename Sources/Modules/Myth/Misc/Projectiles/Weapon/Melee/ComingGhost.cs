using Everglow.Myth.Misc.Dusts;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

internal class ComingGhost : MeleeProj
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override void SetDef()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 30;
		Projectile.extraUpdates = 1;
		Projectile.scale = 1f;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 15;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 80;
		Projectile.height = 80;
		Projectile.tileCollide = false;
		Projectile.friendly = true;
		longHandle = false;
		maxAttackType = 3;
		maxSlashTrailLength = 20;
		shaderType = Commons.MEAC.Enums.MeleeTrailShaderType.ArcBladeTransparentedByZ;
		autoEnd = false;
	}

	private int hasHit = 0;

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		hasHit++;
		if (hasHit > 2)
		{
			return;
		}

		Player player = Main.player[Projectile.owner];
		Vector2 v = new Vector2(0, 6).RotatedByRandom(Math.PI * 2) * 5f;
		Projectile.NewProjectile(null, target.Center - v * 3, v, ModContent.ProjectileType<GhostHit>(), Projectile.damage, Projectile.knockBack, player.whoAmI, Main.rand.NextFloat(-0.05f, 0.05f));
	}

	public override string TrailShapeTex()
	{
		return Commons.ModAsset.Melee_Mod;
	}

	public override string TrailColorTex()
	{
		return "Everglow/Myth/Misc/Projectiles/Weapon/Melee/ComingGhost_meleeColor";
	}

	public override float TrailAlpha(float factor)
	{
		return base.TrailAlpha(factor) * 1.15f;
	}

	public override BlendState TrailBlendState()
	{
		return BlendState.NonPremultiplied;
	}

	public override void End()
	{
		Player player = Main.player[Projectile.owner];
		player.legFrame = new Rectangle(0, 0, player.legFrame.Width, player.legFrame.Height);
		player.fullRotation = 0;
		player.legRotation = 0;
		player.legPosition = Vector2.Zero;
		Projectile.Kill();
		player.GetModPlayer<MEACPlayer>().isUsingMeleeProj = false;
	}

	public override void OnSpawn(IEntitySource source)
	{
		hasHit = 0;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		base.AI();
		TestPlayerDrawer Tplayer = player.GetModPlayer<TestPlayerDrawer>();
		Tplayer.HideLeg = true;
		useSlash = true;
		float timeMul = 1 / player.meleeSpeed;
		if (currantAttackType == 0)
		{
			if (timer < 14 * timeMul)// 前摇
			{
				useSlash = false;
				LockPlayerDir(player);
				float targetRot = -MathHelper.PiOver2 - player.direction * 0.5f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(100, targetRot, +1.2f), 0.4f / timeMul);
				mainAxisDirection += Projectile.DirectionFrom(player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == (int)(14 * timeMul))
			{
				AttSound(SoundID.Item1);
			}

			if (timer > 14 * timeMul && timer < 35 * timeMul)
			{
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.32f / timeMul;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(110, Projectile.rotation, -1.2f, -0.3f * Projectile.spriteDirection), 0.4f / timeMul);
				player.fullRotationOrigin = new Vector2(10, 42);
				player.fullRotation = MathF.Sin((timer - 14 * timeMul) / (25f * timeMul) * MathHelper.Pi) * 0.6f * player.direction;
				player.legRotation = -player.fullRotation;

				Dust d = Dust.NewDustDirect(player.Center + mainAxisDirection * MathF.Sqrt(Main.rand.NextFloat(1f)), 0, 0, ModContent.DustType<Crow>());
				d.scale = Main.rand.NextFloat(2f, 4.5f);
			}
			if (timer > 44 * timeMul)
			{
				hasHit = 0;
				NextAttackType();
			}
		}

		if (currantAttackType == 1)
		{
			if (timer < 9 * timeMul)// 前摇
			{
				useSlash = false;
				LockPlayerDir(player);
				float targetRot = -MathHelper.PiOver2 - player.direction * 0.5f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(100, targetRot, +1.2f), 0.4f / timeMul);
				mainAxisDirection += Projectile.DirectionFrom(player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == (int)(10 * timeMul))
			{
				AttSound(SoundID.Item1);
			}

			if (timer > 9 * timeMul && timer < 42 * timeMul)
			{
				canHit = true;
				Projectile.rotation += Projectile.spriteDirection * 0.17f / timeMul;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(110, Projectile.rotation, 0, 0.3f * Projectile.spriteDirection), 0.4f / timeMul);
				player.fullRotationOrigin = new Vector2(10, 42);
				player.fullRotation = MathF.Sin((timer - 9 * timeMul) / (30f * timeMul) * MathHelper.Pi) * 0.6f * player.direction;
				player.legRotation = -player.fullRotation;

				Dust d = Dust.NewDustDirect(player.Center + mainAxisDirection * MathF.Sqrt(Main.rand.NextFloat(1f)), 0, 0, ModContent.DustType<Crow>());
				d.scale = Main.rand.NextFloat(2f, 4.5f);
			}
			if (timer > 55 * timeMul)
			{
				hasHit = 0;
				NextAttackType();
			}
		}

		if (currantAttackType == 2)
		{
			if (timer < 10 * timeMul)// 前摇
			{
				useSlash = false;
				LockPlayerDir(player);
				float targetRot = -MathHelper.PiOver2 - player.direction * 1.6f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(100, targetRot, +1.2f), 0.4f / timeMul);
				mainAxisDirection += Projectile.DirectionFrom(player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == (int)(10 * timeMul))
			{
				AttSound(SoundID.Item1);
			}

			if (timer > 9 * timeMul && timer < 25 * timeMul)
			{
				canHit = true;
				Projectile.rotation -= Projectile.spriteDirection * 0.45f / timeMul;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(110, Projectile.rotation, -0.2f, 0.3f * Projectile.spriteDirection), 0.4f / timeMul);
				player.fullRotationOrigin = new Vector2(10, 42);
				player.fullRotation = MathF.Sin((timer - 9 * timeMul) / (10f * timeMul) * MathHelper.Pi) * 0.6f * player.direction;
				player.legRotation = -player.fullRotation;
				for (int i = 0; i < 3; i++)
				{
					Dust d = Dust.NewDustDirect(player.Center + mainAxisDirection * MathF.Sqrt(Main.rand.NextFloat(1f)), 0, 0, ModContent.DustType<Crow>());
					d.scale = Main.rand.NextFloat(2f, 4.5f);
				}
			}
			if (timer > 30 * timeMul)
			{
				hasHit = 0;
				NextAttackType();
			}
		}

		if (currantAttackType == 3)
		{
			if (timer < 2 * timeMul)// 前摇
			{
				Projectile.ai[0] = 0;
				Projectile.ai[1] = player.direction;
				Style3StartPoint = player.Center;
				useSlash = false;
				LockPlayerDir(player);
				float targetRot = -MathHelper.PiOver2 - player.direction * 0.5f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(100, targetRot, +1.2f), 0.4f / timeMul);
				mainAxisDirection += Projectile.DirectionFrom(player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
				if (timer == 1)
				{
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<ComingGhost_Shimmer>(), 0, Projectile.knockBack, player.whoAmI, player.direction);
				}
			}
			if (timer == (int)(10 * timeMul))
			{
				AttSound(SoundID.Item1);
			}

			if (timer > 2 * timeMul && timer < 25 * timeMul)
			{
				LockPlayerDir(player);
				canHit = true;
				player.immuneAlpha = 255;
				Projectile.rotation += Projectile.spriteDirection * 0.4f / timeMul;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(110, Projectile.rotation, -1.25f, -0.1f * Projectile.spriteDirection), 0.4f / timeMul);
				player.fullRotationOrigin = new Vector2(10, 42);
				player.fullRotation = MathF.Sin((timer - 16 * timeMul) / (28f * timeMul) * MathHelper.Pi) * 0.6f * Projectile.ai[1];
				player.legRotation = -player.fullRotation;
				float duration = (timer - 2 * timeMul) / (float)(23 * timeMul) * 100f;
				if (duration > 10 * Projectile.ai[0])
				{
					Projectile.ai[0]++;
					if (!Collision.SolidCollision(player.position + new Vector2(35 * Projectile.ai[1], 0), player.width, player.height - 20))
					{
						player.position.X += 35 * Projectile.ai[1];
					}

					Vector2 v0 = new Vector2(0, 14 * Main.rand.NextFloat(0.65f, 1.8f)).RotatedByRandom(MathHelper.TwoPi);
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), Style3StartPoint + new Vector2((60 + duration * 2.5f) * Projectile.ai[1], 0) - v0 * 8, v0, ModContent.ProjectileType<ComingGhost_Slash>(), Projectile.damage, Projectile.knockBack);
				}
			}
			if (timer > 44 * timeMul)
			{
				hasHit = 0;
				NextAttackType();
			}
			if (timer == (int)(40 * timeMul))
			{
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<ComingGhost_Shimmer>(), 0, Projectile.knockBack, player.whoAmI, -player.direction);
			}
		}
	}

	public Vector2 Style3StartPoint = Vector2.zeroVector;

	public override void OnKill(int timeLeft)
	{
		Player player = Main.player[Projectile.owner];
		player.fullRotation = 0;
	}

	public override void DrawSelf(SpriteBatch spriteBatch, Color lightColor, Vector4 diagonal = default, Vector2 drawScale = default, Texture2D glowTexture = null)
	{
		if (currantAttackType == 3)
		{
			return;
		}

		// drawScale = new Vector2(-0.6f, 1.14f);
		base.DrawSelf(spriteBatch, lightColor, diagonal, drawScale, glowTexture);
	}

	public override void DrawTrail(Color color)
	{
		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(slashTrail.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x <= SmoothTrailX.Count - 1; x++)
		{
			SmoothTrail.Add(SmoothTrailX[x]);
		}
		if (slashTrail.Count != 0)
		{
			SmoothTrail.Add(slashTrail.ToArray()[slashTrail.Count - 1]);
		}

		int length = SmoothTrail.Count;
		if (length <= 3)
		{
			return;
		}

		Vector2[] trail = SmoothTrail.ToArray();
		var bars = new List<Vertex2D>();

		for (int i = 0; i < length; i++)
		{
			float factor = i / (length - 1f);
			float w = TrailAlpha(factor);
			Color c0 = Color.White;
			if (i == 0)
			{
				c0 = Color.Transparent;
			}
			bars.Add(new Vertex2D(Projectile.Center + trail[i] * 0.3f * Projectile.scale, c0, new Vector3(factor, 1, 0f)));
			bars.Add(new Vertex2D(Projectile.Center + trail[i] * Projectile.scale, c0, new Vector3(factor, 0, w)));
		}
		bars.Add(new Vertex2D(Projectile.Center + mainAxisDirection * 0.3f * Projectile.scale, Color.White, new Vector3(0, 1, 0f)));
		bars.Add(new Vertex2D(Projectile.Center + mainAxisDirection * Projectile.scale, Color.White, new Vector3(0, 0, 1)));
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

		Effect MeleeTrail = Commons.ModAsset.MeleeTrail.Value;
		MeleeTrail.Parameters["uTransform"].SetValue(model * projection);
		Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(TrailShapeTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

		// Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>(TrailColorTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		MeleeTrail.Parameters["tex1"].SetValue(ModContent.Request<Texture2D>(TrailColorTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
		MeleeTrail.CurrentTechnique.Passes[ShaderTypeName].Apply();

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}

	public override void DrawWarp(VFXBatch spriteBatch)
	{
	}
}
