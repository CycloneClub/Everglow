using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.KelpCurtain.VFXs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee.EvilHalbertBarnacle;

public class EvilHalbertBarnacle_proj_shuttle : TrailingProjectile
{
	public new int Timer = 0;

	/// <summary>
	/// 0: shoot; 1 hit; 2 back; 3 being controlled
	/// </summary>
	public int State = 0;

	public int ParentProjectile = -1;

	public float TrailDuration = 0;

	public float Power = 0f;

	public float ThinnerFactor = 0;

	public Queue<float> ThinnerTrail = new Queue<float>();

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 120000;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
		TrailColor = new Color(0.8f, 0.2f, 0.3f, 0f);
		TrailWidth = 30f;
		SelfLuminous = false;
		TrailTexture = Commons.ModAsset.Trail_8.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_8_black.Value;
		TrailShader = Commons.ModAsset.Trailing.Value;
	}

	public override void AI()
	{
		Timer++;
		if (TimeAfterEntityDestroy >= 0 && TimeAfterEntityDestroy <= 2)
		{
			Projectile.Kill();
		}
		TimeAfterEntityDestroy--;
		if (TimeAfterEntityDestroy >= 0)
		{
			Projectile.velocity *= 0f;
			return;
		}
		Player player = Main.player[Projectile.owner];

		ThinnerTrail.Enqueue(ThinnerFactor);
		if (ThinnerTrail.Count > Projectile.oldPos.Length)
		{
			ThinnerTrail.Dequeue();
		}

		if (State == 0)
		{
			ThinnerFactor = ThinnerFactor * 0.75f + 0.025f;
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * 1;
			for (int v = 0; v < 15; v++)
			{
				if (Collision.SolidCollision(Projectile.position + Projectile.velocity.NormalizeSafe() * v * 5, Projectile.width, Projectile.height))
				{
					Projectile.Center = Projectile.Center + Projectile.velocity.NormalizeSafe() * v * 5;
					Projectile.velocity *= 0;
					State = 1;
					Power = 100;
					Timer = 0;
					CollideEffect();
					break;
				}
			}
			if (Timer > 30)
			{
				Power = 70;
				Projectile.velocity -= (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 35;
				Timer = 0;
				State = 2;
			}
		}
		if (State == 1)
		{
			ThinnerFactor = ThinnerFactor * 0.75f + 0.025f;
			Projectile.friendly = false;
			Timer++;
			if (HasTarget())
			{
				State = 3;
				return;
			}
			if (Power > 0)
			{
				Power--;
			}
			else
			{
				Power = 0;
			}
			if (Timer >= 300)
			{
				Power = 70;
				Projectile.velocity -= (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 35;
				Timer = 0;
				State = 2;
			}
		}
		if (State == 2)
		{
			ThinnerFactor = ThinnerFactor * 0.75f + 0.025f;
			Projectile.friendly = true;
			Power--;
			Projectile.velocity *= 0.9f;
			Vector2 target = player.Center;
			Vector2 toTarget = target - Projectile.Center - Projectile.velocity * 0.2f;
			if (HasTarget())
			{
				State = 3;
				return;
			}
			if (toTarget.Length() < 20)
			{
				Projectile.Kill();
			}
			toTarget = toTarget.NormalizeSafe() * 24;
			toTarget.Y *= 0.5f;
			Projectile.velocity += toTarget * 0.1f;
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
		}
		if (State == 3)
		{
			Projectile.friendly = true;
			Projectile meleeWeapon = default;
			if (ParentProjectile >= 0)
			{
				meleeWeapon = Main.projectile[ParentProjectile];
			}
			if (meleeWeapon == null || !meleeWeapon.active || meleeWeapon.type != ModContent.ProjectileType<EvilHalbertBarnacle_proj>() || meleeWeapon.owner != Projectile.owner)
			{
				State = 2;
				return;
			}
			Power = 70;
			Projectile.velocity *= 0.9f;
			Vector2 target = player.Center;
			EvilHalbertBarnacle_proj melee = meleeWeapon.ModProjectile as EvilHalbertBarnacle_proj;
			if (melee == null)
			{
				State = 2;
				return;
			}
			TrailDuration += Math.Abs(melee.Omega);
			Vector2 offsetTarget = new Vector2(0, 240).RotatedBy(TrailDuration);
			offsetTarget.Y *= 0.35f;
			offsetTarget = offsetTarget.RotatedBy(Math.Sin(Main.time / 24f + Projectile.whoAmI));
			offsetTarget.X *= player.direction;
			target += offsetTarget;
			Vector2 toTarget = target - Projectile.Center - Projectile.velocity;
			if (toTarget.Length() > 300)
			{
				Vector2 v0 = toTarget.NormalizeSafe();
				Vector2 v1 = Projectile.velocity.NormalizeSafe();
				if (Vector2.Dot(v0, v1) < 0)
				{
					Projectile.velocity *= 0.6f;
				}
			}

			float speed = Math.Abs(melee.Omega) * 540 + 12;
			ThinnerFactor = Math.Clamp(Projectile.velocity.Length() / 30f - 1, 0, 1);
			toTarget = toTarget.NormalizeSafe() * speed;
			toTarget.Y *= 0.5f;
			Projectile.velocity += toTarget * 0.1f;
			if (Projectile.velocity.Length() > 15f)
			{
				Vector2 vel = Projectile.velocity * Main.rand.NextFloat(0.5f, 1.15f) * 0.1f;
				var dust = new BarnacleTissueDust
				{
					velocity = vel,
					Active = true,
					Visible = true,
					position = Projectile.Center + Projectile.velocity * 5,
					maxTime = Main.rand.Next(20, 30),
					scale = Main.rand.NextFloat(5f, 30f),
					rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(6f, 8f), Main.rand.NextFloat(1f) },
				};
				Ins.VFXManager.Add(dust);
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
		}
		Lighting.AddLight(Projectile.Center, new Vector3(0.7f, 0.4f, 0.3f) * Power / 70f);
	}

	public void CollideEffect()
	{
		for (int i = 0; i < 16; ++i)
		{
			Vector2 vel = new Vector2(0, 1).RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.5f, 1.15f) * 15f;
			var dust = new BarnacleTissueDust
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = Projectile.Center + Projectile.velocity * 5,
				maxTime = Main.rand.Next(20, 30),
				scale = Main.rand.NextFloat(8f, 45f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(6f, 8f), Main.rand.NextFloat(1f) },
			};
			Ins.VFXManager.Add(dust);
		}
		for (int i = 0; i < 6; ++i)
		{
			Vector2 vel = new Vector2(0, 1).RotateRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.5f, 1.15f) * 35f;
			var dust = new BarnacleTissueDust
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = Projectile.Center + Projectile.velocity * 5,
				maxTime = Main.rand.Next(10, 26),
				scale = Main.rand.NextFloat(60f, 150f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(6f, 8f), Main.rand.NextFloat(1f) },
			};
			Ins.VFXManager.Add(dust);
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float length = Projectile.velocity.Length() / 4f;
		Vector2 norVel = Projectile.velocity.NormalizeSafe();
		for (int i = 0; i <= length; i++)
		{
			Rectangle rec = projHitbox;
			rec.X += (int)(norVel.X * i * 4);
			rec.Y += (int)(norVel.Y * i * 4);
			if (rec.Intersects(targetHitbox))
			{
				return true;
			}
		}
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (State == 0)
		{
			Projectile.velocity *= 0;
			Projectile.Center = target.Center;
			State = 1;
			Power = 100;
			Timer = 0;
			CollideEffect();
		}
	}

	public bool HasTarget()
	{
		foreach (var proj in Main.projectile)
		{
			if (proj != null && proj.active && proj.type == ModContent.ProjectileType<EvilHalbertBarnacle_proj>() && proj.owner == Projectile.owner)
			{
				ParentProjectile = proj.whoAmI;
				return true;
			}
		}
		return false;
	}

	public override void OnKill(int timeLeft) => base.OnKill(timeLeft);

	public override bool PreDraw(ref Color lightColor)
	{
		DrawTrailDark();
		DrawTrailDark();
		DrawTrail();
		if (TimeAfterEntityDestroy <= 0)
		{
			DrawSelf();
		}
		return false;
	}

	public override void DrawTrail()
	{
		var unSmoothPos = new List<Vector2>();
		for (int i = 0; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			unSmoothPos.Add(Projectile.oldPos[i]);
		}
		float smoothIndexRatio = unSmoothPos.Count;
		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(unSmoothPos); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < SmoothTrailX.Count - 1; x++)
		{
			SmoothTrail.Add(SmoothTrailX[x]);
		}
		if (unSmoothPos.Count != 0)
		{
			SmoothTrail.Add(unSmoothPos[unSmoothPos.Count - 1]);
		}
		smoothIndexRatio /= SmoothTrail.Count;
		Vector2 halfSize = new Vector2(Projectile.width, Projectile.height) / 2f;
		var bars = new List<Vertex2D>();
		var bars2 = new List<Vertex2D>();
		var bars3 = new List<Vertex2D>();

		var bars4 = new List<Vertex2D>();
		var bars5 = new List<Vertex2D>();
		var bars6 = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; ++i)
		{
			float mulFac = Timer / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type];
			if (mulFac > 1f)
			{
				mulFac = 1f;
			}
			float factor = i / (float)SmoothTrail.Count * mulFac;
			float width = TrailWidthFunction(factor);
			if (i == 0)
			{
				width = 0;
			}
			int trailThinnerIndex = Math.Clamp((int)(smoothIndexRatio * i), 0, ThinnerTrail.Count - 1);
			float timeValue = (float)Main.time * 0.0005f;
			factor *= 3;
			factor += timeValue;

			Vector2 drawPos = SmoothTrail[i] + halfSize;
			Color drawC = TrailColor;
			if (!SelfLuminous)
			{
				Color lightC = Lighting.GetColor((drawPos / 16f).ToPoint());
				drawC.R = (byte)(lightC.R * drawC.R / 255f);
				drawC.G = (byte)(lightC.G * drawC.G / 255f);
				drawC.B = (byte)(lightC.B * drawC.B / 255f);
			}
			bars.Add(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 2f / 3f) * TrailWidth, drawC, new Vector3(factor, 1, width));
			bars.Add(drawPos, drawC, new Vector3(factor, 0.5f, width));
			bars2.Add(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 1f / 3f) * TrailWidth, drawC, new Vector3(factor, 1, width));
			bars2.Add(drawPos, drawC, new Vector3(factor, 0.5f, width));
			bars3.Add(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 0f / 3f) * TrailWidth, drawC, new Vector3(factor, 1, width));
			bars3.Add(drawPos, drawC, new Vector3(factor, 0.5f, width));

			float trailWidth2 = TrailWidth * 0.4f;
			float oldThin = ThinnerTrail.ToArray()[trailThinnerIndex];
			Color trailColor2 = Color.Lerp(TrailColor * 0.3f, new Color(0.2f, 0.8f, 1, 0), 1 - Math.Clamp((oldThin - 0.5f) * 2, 0, 1));
			trailColor2 *= 2;
			width *= MathF.Pow(oldThin, 3f);
			bars4.Add(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 2f / 3f) * trailWidth2, trailColor2, new Vector3(factor, 1, width));
			bars4.Add(drawPos, trailColor2, new Vector3(factor, 0.5f, width));
			bars5.Add(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 1f / 3f) * trailWidth2, trailColor2, new Vector3(factor, 1, width));
			bars5.Add(drawPos, trailColor2, new Vector3(factor, 0.5f, width));
			bars6.Add(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 0f / 3f) * trailWidth2, trailColor2, new Vector3(factor, 1, width));
			bars6.Add(drawPos, trailColor2, new Vector3(factor, 0.5f, width));
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect effect = TrailShader;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.Textures[0] = TrailTexture;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		if (bars2.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
		}
		if (bars3.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars3.ToArray(), 0, bars3.Count - 2);
		}

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_7.Value;
		if (bars4.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars4.ToArray(), 0, bars4.Count - 2);
		}
		if (bars5.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars5.ToArray(), 0, bars5.Count - 2);
		}
		if (bars6.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars6.ToArray(), 0, bars6.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public void DrawTrailDark()
	{
		var unSmoothPos = new List<Vector2>();
		for (int i = 0; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			unSmoothPos.Add(Projectile.oldPos[i]);
		}
		float smoothIndexRatio = unSmoothPos.Count;
		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(unSmoothPos); // 平滑
		var SmoothTrail = new List<Vector2>();
		for (int x = 0; x < SmoothTrailX.Count - 1; x++)
		{
			SmoothTrail.Add(SmoothTrailX[x]);
		}
		if (unSmoothPos.Count != 0)
		{
			SmoothTrail.Add(unSmoothPos[unSmoothPos.Count - 1]);
		}
		Vector2 halfSize = new Vector2(Projectile.width, Projectile.height) / 2f;
		var bars = new List<Vertex2D>();
		var bars2 = new List<Vertex2D>();
		var bars3 = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; ++i)
		{
			float mulFac = Timer / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type];
			if (mulFac > 1f)
			{
				mulFac = 1f;
			}
			float factor = i / (float)SmoothTrail.Count * mulFac;
			float width = TrailWidthFunction(factor);
			if (i == 0)
			{
				width = 0;
			}
			float timeValue = (float)Main.time * 0.0005f;
			factor *= 3;
			factor += timeValue;

			Vector2 drawPos = SmoothTrail[i] + halfSize;
			Color drawC = new Color(1, 1, 1, 1f);
			bars.Add(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 2f / 3f) * TrailWidth, drawC, new Vector3(factor, 1, width));
			bars.Add(drawPos, drawC, new Vector3(factor, 0.5f, width));
			bars2.Add(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 1f / 3f) * TrailWidth, drawC, new Vector3(factor, 1, width));
			bars2.Add(drawPos, drawC, new Vector3(factor, 0.5f, width));
			bars3.Add(drawPos + new Vector2(0, 1).RotatedBy(MathHelper.TwoPi * 0f / 3f) * TrailWidth, drawC, new Vector3(factor, 1, width));
			bars3.Add(drawPos, drawC, new Vector3(factor, 0.5f, width));
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect effect = TrailShader;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_8_black.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		if (bars2.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
		}
		if (bars3.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars3.ToArray(), 0, bars3.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public override void DrawSelf()
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		var texGlow = ModAsset.EvilHalbertBarnacle_proj_shuttle_glow.Value;
		var texBloom = ModAsset.EvilHalbertBarnacle_proj_shuttle_bloom.Value;
		if (Power > 50)
		{
			float value = Power - 50;
			value /= 50f;
			Color drawColor = new Color(value, value, value, 0);
			Main.spriteBatch.Draw(texBloom, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, texBloom.Size() / 2f, 1f, SpriteEffects.None, 0);
		}
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, Lighting.GetColor(Projectile.Center.ToTileCoordinates()), Projectile.rotation, texMain.Size() / 2f, 1f, SpriteEffects.None, 0);
		if (Power > 0)
		{
			float value = Power / 100f;
			Color drawColor = new Color(value, value, value, 0);
			Main.spriteBatch.Draw(texGlow, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, texGlow.Size() / 2f, 1f, SpriteEffects.None, 0);
		}
	}
}
