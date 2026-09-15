using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Mono.Cecil;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

public class QuenchingBladeProj : MeleeProj
{
	public override void SetDef()
	{
		maxAttackType = 4;
		maxSlashTrailLength = 20;
		longHandle = true;
		autoEnd = true;
		canLongLeftClick = true;
		Omega = 0;
	}

	public override string TrailShapeTex()
	{
		return Commons.ModAsset.Melee_Mod;
	}

	public override string TrailColorTex()
	{
		return ModAsset.HeatMap_QuenchingBladeProj_Mod;
	}

	public override float TrailAlpha(float factor)
	{
		return base.TrailAlpha(factor) * 1.3f;
	}

	public override BlendState TrailBlendState()
	{
		return BlendState.NonPremultiplied;
	}

	public override void DrawSelf(SpriteBatch spriteBatch, Color lightColor, Vector4 diagonal = default, Vector2 drawScale = default, Texture2D glowTexture = null)
	{
		if (diagonal == default)
		{
			diagonal = new Vector4(0, 1, 1, 0);
		}
		if (drawScale == default)
		{
			drawScale = new Vector2(0, 1);
			if (longHandle)
			{
				drawScale = new Vector2(-0.3f, 1);
			}
			drawScale *= drawScaleFactor;
		}
		Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
		Texture2D texGlow = ModAsset.QuenchingBladeProj_glow.Value;
		Vector2 drawCenter = Projectile.Center - Main.screenPosition;

		SpriteBatchState sBS = spriteBatch.GetState().Value;
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		DrawVertexByTwoLine(tex, lightColor, diagonal.XY(), diagonal.ZW(), drawCenter + mainAxisDirection * drawScale.X, drawCenter + mainAxisDirection * drawScale.Y);
		DrawVertexByTwoLine(texGlow, new Color(1f, 1f, 1f, 0), diagonal.XY(), diagonal.ZW(), drawCenter + mainAxisDirection * drawScale.X, drawCenter + mainAxisDirection * drawScale.Y);

		spriteBatch.End();
		spriteBatch.Begin(sBS);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		// 伤害倍率
		float ShakeStrength = 0.2f;
		if (currantAttackType == 3)
		{
			modifiers.FinalDamage *= 1.4f;
			ShakeStrength = 1f;
		}
		modifiers.HitDirectionOverride = target.Center.X > Main.player[Projectile.owner].Center.X ? 1 : -1;
		ShakerManager.AddShaker(Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.Pi), 10 * ShakeStrength, 20f, 120, 0.9f, 0.8f, 30);
	}

	public float Omega;

	public override void Attack()
	{
		Player player = Main.player[Projectile.owner];
		TestPlayerDrawer Tplayer = player.GetModPlayer<TestPlayerDrawer>();
		Tplayer.HideLeg = true;
		float timeMul = 1f / player.meleeSpeed;

		useSlash = true;
		if (currantAttackType == 0 || currantAttackType == 2)
		{
			if (timer < 20 * timeMul)
			{
				useSlash = false;
				LockPlayerDir(Player);
				float targetRot = -MathHelper.PiOver2 - Player.direction * 1.2f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(180, targetRot, -1.2f), 0.15f);
				mainAxisDirection += Projectile.DirectionFrom(Player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == (int)(8 * timeMul))
			{
				PlayAttackSound();
			}

			if (timer > 20 * timeMul && timer < 75 * timeMul)
			{
				Lighting.AddLight(Projectile.Center + mainAxisDirection, 0.24f, 0.06f, 0f);
				canHit = true;
				if (timer < 30 * timeMul)
				{
					Omega += 0.04f / timeMul;
				}
				if (timer > 40 * timeMul)
				{
					Omega *= 0.5f / MathF.Log(timeMul * MathHelper.E);
				}
				Projectile.rotation += Projectile.spriteDirection * Omega;
				mainAxisDirection = Vector2Elipse(250, Projectile.rotation, -1.2f, 0, 1000);
				var lineStart = Vector2.zeroVector;
				var lineEnd = Vector2.zeroVector;
				if (timer > 25 * timeMul && timer < 43 * timeMul)
				{
					lineStart = ProjCenter_WithoutGravDir + Vector2Elipse(250, Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f), -1.2f, 0, 1000) * Projectile.scale * 0.6f;
					lineEnd = ProjCenter_WithoutGravDir + Vector2Elipse(250, Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f), -1.2f, 0, 1000) * Projectile.scale * 1.2f;
					for (int k = 0; k < 4 / timeMul; k++)
					{
						var dustVFX = new FlameDust0
						{
							velocity = Vector2.zeroVector,
							Active = true,
							Visible = true,
							position = Vector2.Lerp(lineEnd, lineStart, MathF.Sqrt(Main.rand.NextFloat())),
							maxTime = Main.rand.Next(6, 20),
							scale = Main.rand.NextFloat(15, 60),
							rotation = MathHelper.PiOver4 * 3,
							MyOwner = player,
							ai = new float[] { Main.rand.Next(3), 0, 0 },
						};
						Ins.VFXManager.Add(dustVFX);
					}
					for (int k = 0; k < 4 / timeMul; k++)
					{
						lineStart = ProjCenter_WithoutGravDir + Vector2Elipse(250, Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f), -1.2f, 0, 1000) * Projectile.scale * 0.9f;
						lineEnd = ProjCenter_WithoutGravDir + Vector2Elipse(250, Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f), -1.2f, 0, 1000) * Projectile.scale * 1.2f;
						var dustVFX = new FlameDust1
						{
							velocity = new Vector2(0, -4),
							Active = true,
							Visible = true,
							position = Vector2.Lerp(lineEnd, lineStart, MathF.Sqrt(Main.rand.NextFloat())),
							maxTime = Main.rand.Next(6, 20),
							scale = Main.rand.NextFloat(5, 20),
							rotation = MathHelper.PiOver4 * 3,
							MyOwner = player,
							ai = new float[] { Main.rand.Next(3), 0, 0 },
						};
						Ins.VFXManager.Add(dustVFX);
					}
				}
			}
			if (timer > 70 * timeMul)
			{
				NextAttackType();
			}
			else if (timer > 1)
			{
				float BodyRotation = (float)Math.Sin((timer - 10) / 30d * Math.PI) * 0.2f * -player.direction * player.gravDir;
				player.fullRotation = BodyRotation;
				player.fullRotationOrigin = new Vector2(player.Hitbox.Width / 2f, player.gravDir == -1 ? 0 : player.Hitbox.Height);
				player.legRotation = -BodyRotation;
				player.legPosition = (new Vector2(player.Hitbox.Width / 2f, player.Hitbox.Height) - player.fullRotationOrigin).RotatedBy(-BodyRotation);
				Tplayer.HeadRotation = -BodyRotation;
			}
		}
		if (currantAttackType == 1)
		{
			if (timer < 20 * timeMul)
			{
				useSlash = false;
				LockPlayerDir(Player);
				float targetRot = -MathHelper.PiOver2 - Player.direction * 2f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(180, targetRot, -1.2f), 0.15f);
				mainAxisDirection += Projectile.DirectionFrom(Player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == (int)(8 * timeMul))
			{
				PlayAttackSound();
			}

			if (timer > 20 * timeMul && timer < 75 * timeMul)
			{
				Lighting.AddLight(Projectile.Center + mainAxisDirection, 0.24f, 0.06f, 0f);
				canHit = true;
				if (timer < 30 * timeMul)
				{
					Omega += 0.04f / timeMul;
				}
				if (timer > 40 * timeMul)
				{
					Omega *= 0.5f / MathF.Log(timeMul * MathHelper.E);
				}
				Projectile.rotation -= Projectile.spriteDirection * Omega;
				mainAxisDirection = Vector2Elipse(250, Projectile.rotation, -1.2f, 0, 1000);
				var lineStart = Vector2.zeroVector;
				var lineEnd = Vector2.zeroVector;
				if (timer > 25 * timeMul && timer < 43 * timeMul)
				{
					lineStart = ProjCenter_WithoutGravDir + Vector2Elipse(250, Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f), -1.2f, 0, 1000) * Projectile.scale * 0.6f;
					lineEnd = ProjCenter_WithoutGravDir + Vector2Elipse(250, Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f), -1.2f, 0, 1000) * Projectile.scale * 1.2f;
					for (int k = 0; k < 4 / timeMul; k++)
					{
						var dustVFX = new FlameDust0
						{
							velocity = Vector2.zeroVector,
							Active = true,
							Visible = true,
							position = Vector2.Lerp(lineEnd, lineStart, MathF.Sqrt(Main.rand.NextFloat())),
							maxTime = Main.rand.Next(6, 20),
							scale = Main.rand.NextFloat(15, 60),
							rotation = MathHelper.PiOver4 * 3,
							MyOwner = player,
							ai = new float[] { Main.rand.Next(3), 0, 0 },
						};
						Ins.VFXManager.Add(dustVFX);
					}
					for (int k = 0; k < 4 / timeMul; k++)
					{
						lineStart = ProjCenter_WithoutGravDir + Vector2Elipse(250, Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f), -1.2f, 0, 1000) * Projectile.scale * 0.9f;
						lineEnd = ProjCenter_WithoutGravDir + Vector2Elipse(250, Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f), -1.2f, 0, 1000) * Projectile.scale * 1.2f;
						var dustVFX = new FlameDust1
						{
							velocity = new Vector2(0, -4),
							Active = true,
							Visible = true,
							position = Vector2.Lerp(lineEnd, lineStart, MathF.Sqrt(Main.rand.NextFloat())),
							maxTime = Main.rand.Next(6, 20),
							scale = Main.rand.NextFloat(5, 20),
							rotation = MathHelper.PiOver4 * 3,
							MyOwner = player,
							ai = new float[] { Main.rand.Next(3), 0, 0 },
						};
						Ins.VFXManager.Add(dustVFX);
					}
				}
			}
			if (timer > 70 * timeMul)
			{
				NextAttackType();
			}
			else if (timer > 1)
			{
				float BodyRotation = (float)Math.Sin((timer - 10) / 30d * Math.PI) * 0.2f * player.direction * player.gravDir;
				player.fullRotation = BodyRotation;
				player.fullRotationOrigin = new Vector2(player.Hitbox.Width / 2f, player.gravDir == -1 ? 0 : player.Hitbox.Height);
				player.legRotation = -BodyRotation;
				player.legPosition = (new Vector2(player.Hitbox.Width / 2f, player.Hitbox.Height) - player.fullRotationOrigin).RotatedBy(-BodyRotation);
				Tplayer.HeadRotation = -BodyRotation;
			}
		}
		if (currantAttackType == 3)
		{
			if (timer < 20 * timeMul)
			{
				useSlash = false;
				LockPlayerDir(Player);
				float targetRot = -MathHelper.PiOver2 - Player.direction * 2f;
				mainAxisDirection = Vector2.Lerp(mainAxisDirection, Vector2Elipse(180, targetRot, -1.2f), 0.15f);
				mainAxisDirection += Projectile.DirectionFrom(Player.Center) * 3;
				Projectile.rotation = mainAxisDirection.ToRotation();
			}
			if (timer == (int)(8 * timeMul))
			{
				PlayAttackSound();
			}
			if (timer == (int)(24 * timeMul))
			{
				player.velocity.Y -= 12f;
			}
			if (timer > 20 * timeMul && timer < 75 * timeMul)
			{
				Lighting.AddLight(Projectile.Center + mainAxisDirection, 0.24f, 0.06f, 0f);
				canHit = true;
				if (timer < 30 * timeMul)
				{
					Omega += 0.04f / timeMul;
				}
				if (timer > 40 * timeMul)
				{
					Omega *= 0.5f / MathF.Log(timeMul * MathHelper.E);
				}
				Projectile.rotation -= Projectile.spriteDirection * Omega;
				mainAxisDirection = Vector2Elipse(250, Projectile.rotation, -0f, 0, 1000);

				if (timer > 25 * timeMul && timer < 43 * timeMul)
				{
					for (int k = 0; k < 8 / timeMul; k++)
					{
						var lineStart = ProjCenter_WithoutGravDir + Vector2Elipse(250, Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f), 0, 0, 1000) * Projectile.scale * 0.9f;
						var lineEnd = ProjCenter_WithoutGravDir + Vector2Elipse(250, Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f), 0, 0, 1000) * Projectile.scale * 1.2f;
						var dustVFX = new FlameDust0
						{
							velocity = Vector2.zeroVector,
							Active = true,
							Visible = true,
							position = Vector2.Lerp(lineEnd, lineStart, MathF.Sqrt(Main.rand.NextFloat())),
							maxTime = Main.rand.Next(6, 20),
							scale = Main.rand.NextFloat(15, 80),
							rotation = MathHelper.PiOver4 * 3,
							MyOwner = player,
							ai = new float[] { Main.rand.Next(3), 0, 0 },
						};
						Ins.VFXManager.Add(dustVFX);
					}
					for (int k = 0; k < 8 / timeMul; k++)
					{
						var lineStart = ProjCenter_WithoutGravDir + Vector2Elipse(250, Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f), 0, 0, 1000) * Projectile.scale * 0.9f;
						var lineEnd = ProjCenter_WithoutGravDir + Vector2Elipse(250, Projectile.rotation + Main.rand.NextFloat(-0.5f, 0.5f), 0, 0, 1000) * Projectile.scale * 1.2f;
						var dustVFX = new FlameDust1
						{
							velocity = new Vector2(0, -4),
							Active = true,
							Visible = true,
							position = Vector2.Lerp(lineEnd, lineStart, MathF.Sqrt(Main.rand.NextFloat())),
							maxTime = Main.rand.Next(6, 20),
							scale = Main.rand.NextFloat(5, 20),
							rotation = MathHelper.PiOver4 * 3,
							MyOwner = player,
							ai = new float[] { Main.rand.Next(3), 0, 0 },
						};
						Ins.VFXManager.Add(dustVFX);
					}
				}
			}
			if (timer == 75 * timeMul)
			{
				Omega = 0.2f / timeMul;
			}
			if (timer > 75 * timeMul && timer < 90 * timeMul)
			{
				Omega *= 0.75f / MathF.Log(timeMul * MathHelper.E);
				Projectile.rotation -= Projectile.spriteDirection * Omega;
				mainAxisDirection = Vector2Elipse(250, Projectile.rotation, -0f, 0, 1000);
			}
			if (timer > 85 * timeMul)
			{
				Omega *= 0;
				NextAttackType();
			}
			else if (timer > 1)
			{
				float BodyRotation = (float)Math.Sin((timer - 10) / 30d * Math.PI) * 0.2f * player.direction * player.gravDir;
				player.fullRotation = BodyRotation;
				player.fullRotationOrigin = new Vector2(player.Hitbox.Width / 2f, player.gravDir == -1 ? 0 : player.Hitbox.Height);
				player.legRotation = -BodyRotation;
				player.legPosition = (new Vector2(player.Hitbox.Width / 2f, player.Hitbox.Height) - player.fullRotationOrigin).RotatedBy(-BodyRotation);
				Tplayer.HeadRotation = -BodyRotation;
			}
		}
		if (currantAttackType == 4)
		{
			if (timer == 1)
			{
				float dir = -3.5f;
				if (player.direction == -1)
				{
					dir = 3.5f;
				}
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<QuenchingBladeProj_Smash>(), Projectile.damage, Projectile.knockBack * 10, Projectile.owner, 1.2f, dir);
			}
			if (timer > 110 * timeMul)
			{
				NextAttackType();
			}
		}
	}

	public void PlayAttackSound()
	{
		switch (Main.rand.Next(4))
		{
			case 0:
				AttSound(new SoundStyle(ModAsset.QuenchingBlade_attack_1_Mod));
				break;
			case 1:
				AttSound(new SoundStyle(ModAsset.QuenchingBlade_attack_2_Mod));
				break;
			case 2:
				AttSound(new SoundStyle(ModAsset.QuenchingBlade_attack_3_Mod));
				break;
			case 3:
				AttSound(new SoundStyle(ModAsset.QuenchingBlade_attack_4_Mod));
				break;
		}
	}

	public override void DrawTrail(Color color)
	{
		List<Vector2> smoothTrail_current = GraphicsUtils.CatmullRom(slashTrail.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < smoothTrail_current.Count - 1; x++)
		{
			Vector2 vec = smoothTrail_current[x];

			SmoothTrail.Add(Vector2.Normalize(vec) * (vec.Length() + disFromPlayer));
		}
		if (slashTrail.Count != 0)
		{
			Vector2 vec = slashTrail.ToArray()[slashTrail.Count - 1];

			SmoothTrail.Add(Vector2.Normalize(vec) * (vec.Length() + disFromPlayer));
		}
		Vector2 center = Projectile.Center - Vector2.Normalize(mainAxisDirection) * disFromPlayer;
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
			if (i == length - 1)
			{
				factor = 0;
			}
			float w = TrailAlpha(factor);
			Color drawColor = Color.White;
			int invisible = 0;

			// Main.NewText(length);
			if (timer < 24)
			{
				invisible = length;
			}
			if (timer < 44 && timer >= 24)
			{
				invisible = (int)Utils.Lerp(length, 0, MathF.Pow((timer - 24) / 20f, 2));
			}
			if (i < invisible)
			{
				w = 0;
			}
			if (!longHandle)
			{
				bars.Add(new Vertex2D(center + trail[i] * 0.15f * Projectile.scale, drawColor, new Vector3(factor, 1, 0f)));
				bars.Add(new Vertex2D(center + trail[i] * Projectile.scale, drawColor, new Vector3(factor, 0, w)));
			}
			else
			{
				bars.Add(new Vertex2D(center + trail[i] * 0.3f * Projectile.scale, drawColor, new Vector3(factor, 1, 0f)));
				bars.Add(new Vertex2D(center + trail[i] * Projectile.scale, drawColor, new Vector3(factor, 0, w)));
			}
		}

		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
		Effect meleetrail = Commons.ModAsset.MeleeTrail.Value;
		meleetrail.Parameters["uTransform"].SetValue(model * projection);
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Melee2.Value;
		meleetrail.Parameters["tex1"].SetValue(Commons.ModAsset.HeatMap_Shadow.Value);
		meleetrail.CurrentTechnique.Passes[0].Apply();

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, TrailBlendState(), SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone);
		meleetrail.Parameters["uTransform"].SetValue(model * projection);
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Melee.Value;
		meleetrail.Parameters["tex1"].SetValue(ModContent.Request<Texture2D>(TrailColorTex(), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
		meleetrail.CurrentTechnique.Passes[0].Apply();

		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if (currantAttackType == 4)
		{
			return false;
		}
		DrawSelf(Main.spriteBatch, lightColor);
		DrawTrail(lightColor);
		return false;
	}

	public override void End()
	{
		Player player = Main.player[Projectile.owner];
		TestPlayerDrawer Tplayer = player.GetModPlayer<TestPlayerDrawer>();
		player.legFrame = new Rectangle(0, 0, player.legFrame.Width, player.legFrame.Height);
		player.fullRotation = 0;
		player.legRotation = 0;
		Tplayer.HeadRotation = 0;
		Tplayer.HideLeg = false;
		player.legPosition = Vector2.Zero;
		Projectile.Kill();
		player.GetModPlayer<MEACPlayer>().isUsingMeleeProj = false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.OnFire, 600);
	}

	public override void DrawWarp(VFXBatch spriteBatch)
	{
		if (currantAttackType == 4)
		{
			return;
		}
		List<Vector2> smoothTrail_current = GraphicsUtils.CatmullRom(slashTrail.ToList()); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < smoothTrail_current.Count - 1; x++)
		{
			Vector2 vec = smoothTrail_current[x];

			SmoothTrail.Add(Vector2.Normalize(vec) * (vec.Length() + disFromPlayer));
		}
		if (slashTrail.Count != 0)
		{
			Vector2 vec = slashTrail.ToArray()[slashTrail.Count - 1];

			SmoothTrail.Add(Vector2.Normalize(vec) * (vec.Length() + disFromPlayer));
		}
		Vector2 center = Projectile.Center - Vector2.Normalize(mainAxisDirection) * disFromPlayer;
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
			float w = 1f;
			int invisible = 0;
			if (timer < 22)
			{
				invisible = length;
			}
			if (timer < 42 && timer >= 22)
			{
				invisible = (int)Utils.Lerp(length, 0, MathF.Pow((timer - 22) / 20f, 2));
			}
			if (i < invisible)
			{
				w = 0;
			}
			if (i < 20)
			{
				w *= i / 20f;
			}
			if (i > length - 10)
			{
				w *= (length - i - 1) / 10f;
			}
			w *= 0.1f;
			float d = trail[i].ToRotation() + 3.14f + 1.57f;
			if (d > 6.28f)
			{
				d -= 6.28f;
			}

			float dir = d / MathHelper.TwoPi;

			float dir1 = dir;
			if (i > 0)
			{
				float d1 = trail[i - 1].ToRotation() + 3.14f + 1.57f;
				if (d1 > 6.28f)
				{
					d1 -= 6.28f;
				}

				dir1 = d1 / MathHelper.TwoPi;
			}
			float outer = 1.1f;
			float inner = 0.3f;
			if (dir - dir1 > 0.5)
			{
				var midValue = (1 - dir) / (1 - dir + dir1);
				var midPoint = midValue * trail[i] + (1 - midValue) * trail[i - 1];
				midPoint.X = 0;
				var oldFactor = (i - 1) / (length - 1f);
				var midFactor = midValue * factor + (1 - midValue) * oldFactor;
				bars.Add(new Vertex2D(center - Main.screenPosition + midPoint * Projectile.scale * inner, new Color(0, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(center - Main.screenPosition + midPoint * Projectile.scale * outer, new Color(0, w, 0, 1), new Vector3(midFactor, 0, 1)));
				bars.Add(new Vertex2D(center - Main.screenPosition + midPoint * Projectile.scale * inner, new Color(1, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(center - Main.screenPosition + midPoint * Projectile.scale * outer, new Color(1, w, 0, 1), new Vector3(midFactor, 0, 1)));
			}
			if (dir1 - dir > 0.5)
			{
				var midValue = (1 - dir1) / (1 - dir1 + dir);
				var midPoint = midValue * trail[i - 1] + (1 - midValue) * trail[i];
				midPoint.X = 0;
				var oldFactor = (i - 1) / (length - 1f);
				var midFactor = midValue * oldFactor + (1 - midValue) * factor;
				bars.Add(new Vertex2D(center - Main.screenPosition + midPoint * Projectile.scale * inner, new Color(1, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(center - Main.screenPosition + midPoint * Projectile.scale * outer, new Color(1, w, 0, 1), new Vector3(midFactor, 0, 1)));
				bars.Add(new Vertex2D(center - Main.screenPosition + midPoint * Projectile.scale * inner, new Color(0, w, 0, 1), new Vector3(midFactor, 1, 1)));
				bars.Add(new Vertex2D(center - Main.screenPosition + midPoint * Projectile.scale * outer, new Color(0, w, 0, 1), new Vector3(midFactor, 0, 1)));
			}
			bars.Add(new Vertex2D(center - Main.screenPosition + trail[i] * Projectile.scale * inner, new Color(dir, w, 0, 1), new Vector3(factor, 1, w)));
			bars.Add(new Vertex2D(center - Main.screenPosition + trail[i] * Projectile.scale * outer, new Color(dir, w, 0, 1), new Vector3(factor, 0, w)));
		}

		spriteBatch.Draw(Commons.ModAsset.Noise_melting_H.Value, bars, PrimitiveType.TriangleStrip);
	}
}
