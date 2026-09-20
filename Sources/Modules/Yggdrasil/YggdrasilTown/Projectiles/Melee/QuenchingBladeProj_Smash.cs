using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework.Media;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

public class QuenchingBladeProj_Smash : ModProjectile, IWarpProjectile_warpStyle2, IBloomProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override string Texture => ModAsset.QuenchingBladeProj_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 120;
		Projectile.extraUpdates = 0;

		Projectile.localNPCHitCooldown = 60;
		Projectile.usesLocalNPCImmunity = true;

		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 180000;
	}

	public Queue<Vector3> OldPosSpace = new Queue<Vector3>();
	public Queue<Vector2> FallingPos = new Queue<Vector2>();
	public Vector2 FallingMove;
	public Vector2 FallingVel;
	public Vector3 SpacePos;
	public Vector3 RotatedAxis;
	public float Omega = 0;
	public Vector2 DeltaVelocity = default;
	public int FirstDirection = 0;
	public bool HitTile = false;

	public override void OnSpawn(IEntitySource source)
	{
		RotatedAxis = new Vector3(0, 0, 5);
		Vector2 v0 = new Vector2(0, -270) * Projectile.ai[0];
		SpacePos = new Vector3(v0, 0);
		SpacePos = RodriguesRotate(SpacePos, RotatedAxis, Projectile.ai[1]);
		FallingMove = Vector2.zeroVector;
		FallingVel = Vector2.zeroVector;
		Omega = -0.05f;
		HitTile = false;
	}

	public override bool PreAI()
	{
		if (Projectile.timeLeft > 120)
		{
			return false;
		}
		return base.PreAI();
	}

	public override bool ShouldUpdatePosition()
	{
		return true;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		float timeMul = 1f / player.meleeSpeed;
		if (FirstDirection == 0)
		{
			FirstDirection = player.direction;
			Projectile.spriteDirection = player.direction;
		}
		OldPosSpace.Enqueue(SpacePos);
		FallingPos.Enqueue(FallingMove);
		if (!HitTile)
		{
			player.Center = Projectile.Center + FallingMove;
		}
		FallingMove += FallingVel;
		if (OldPosSpace.Count > 90)
		{
			OldPosSpace.Dequeue();
		}
		if (FallingPos.Count > 90)
		{
			FallingPos.Dequeue();
		}
		Vector3 delta0 = SpacePos;
		if (Projectile.spriteDirection == -1)
		{
			Omega *= Projectile.spriteDirection;
			Projectile.spriteDirection = 1;
		}
		SpacePos = RodriguesRotate(SpacePos, RotatedAxis, Omega);
		if (Projectile.timeLeft < 90)
		{
			Omega = 0;

			if (!TileUtils.PlatformCollision(Projectile.Center + FallingMove - new Vector2(30), 60, 60))
			{
				FallingVel.Y += 1.5f;
			}
			else
			{
				if (!HitTile)
				{
					SmashEffect();
					player.velocity *= 0;
					HitTile = true;
				}
				FallingVel.Y *= 0;
			}
		}
		delta0 = SpacePos - delta0;
		Omega *= 0.9f;
		if (Projectile.timeLeft == 114)
		{
			Projectile.friendly = true;
		}
		if (Projectile.timeLeft == 110)
		{
			Projectile.extraUpdates = 8;
		}
		if (Projectile.timeLeft > 114)
		{
			Omega += 0.07f * MathF.Sign(Omega);
		}
		DeltaVelocity = new Vector2(delta0.X, delta0.Y);
		if (!HitTile)
		{
			if (SmoothTrail.Count > 2)
			{
				int stCount = SmoothTrail.Count;
				stCount = Math.Clamp(stCount, 2, 36);
				for (int k = 0; k < 8; k++)
				{
					int index = Main.rand.Next(stCount - 1) + 2;
					var lineStart = Projectile.Center + SmoothTrail[^index] * 0.6f;
					var lineEnd = Projectile.Center + SmoothTrail[^(index - 1)] * 1.2f;
					if (FallingPos.Count > 2)
					{
						lineStart += FallingPos.ToArray()[^1];
						lineEnd += FallingPos.ToArray()[^2];
					}
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
						ai = new float[] { Main.rand.Next(3), 1, 0 },
					};
					Ins.VFXManager.Add(dustVFX);
				}
				for (int k = 0; k < 8; k++)
				{
					int index = Main.rand.Next(stCount - 1) + 2;
					var lineStart = Projectile.Center + SmoothTrail[^index] * 0.6f;
					var lineEnd = Projectile.Center + SmoothTrail[^(index - 1)] * 1.2f;
					if (FallingPos.Count > 2)
					{
						lineStart += FallingPos.ToArray()[^1];
						lineEnd += FallingPos.ToArray()[^2];
					}
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
						ai = new float[] { Main.rand.Next(3), 1, 0 },
					};
					Ins.VFXManager.Add(dustVFX);
				}
			}
		}
	}

	public void SmashEffect()
	{
		Player player = Main.player[Projectile.owner];
		ShakerManager.AddShaker(Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.Pi), 120, 30f, 200, 0.9f, 0.8f, 150);
		switch (Main.rand.Next(2))
		{
			case 0:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.QuenchingBlade_superattack_1_Mod), Projectile.Center);
				break;
			case 1:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.QuenchingBlade_superattack_2_Mod), Projectile.Center);
				break;
		}
		var hitVFX = new QuenchingBlade_SmashVFX
		{
			Active = true,
			Visible = true,
			Position = Projectile.Center + FallingMove + new Vector2(240 * FirstDirection, 30),
			Direction = FirstDirection,
		};
		Ins.VFXManager.Add(hitVFX);

		for (int g = 0; g < 24; g++)
		{
			Vector2 newVelocity = new Vector2(0, -(Main.rand.NextFloat(12f, 25f) + g * 8)).RotatedBy(Main.rand.NextFloat(-0.04f, 0.14f) + g / 24f * FirstDirection);
			Vector2 pos = Projectile.Center + FallingMove + new Vector2((60 + 12 * g) * FirstDirection, 3) - newVelocity * 1;
			var somg = new QuenchingBladeSmog
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = pos,
				maxTime = Main.rand.Next(15, 48),
				scale = Main.rand.NextFloat(50f, 120f),
				ai = new float[] { 0, 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int k = 0; k < 24; k++)
		{
			Vector2 pos = Projectile.Center + FallingMove + new Vector2((60 + 9 * k) * FirstDirection, 3);
			var dustVFX = new FlameDust0
			{
				velocity = Vector2.zeroVector,
				Active = true,
				Visible = true,
				position = pos,
				maxTime = Main.rand.Next(16, 34),
				scale = Main.rand.NextFloat(15, 80),
				rotation = MathHelper.PiOver4 * 3,
				MyOwner = player,
				ai = new float[] { Main.rand.Next(3), 1, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}
		for (int k = 0; k < 48; k++)
		{
			Vector2 pos = Projectile.Center + FallingMove + new Vector2((60 + 6 * k) * FirstDirection, 3);
			var dustVFX = new FlameDust1
			{
				velocity = Vector2.zeroVector,
				Active = true,
				Visible = true,
				position = pos,
				maxTime = Main.rand.Next(16, 34),
				scale = Main.rand.NextFloat(15, 80),
				rotation = MathHelper.PiOver4 * 3,
				MyOwner = player,
				ai = new float[] { Main.rand.Next(3), 1, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}
		Projectile.timeLeft = 70;
		Projectile.extraUpdates = 2;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers.FinalDamage *= 1.7f;
		modifiers.HitDirectionOverride = target.Center.X > Main.player[Projectile.owner].Center.X ? 1 : -1;
		ShakerManager.AddShaker(Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.Pi), 20, 20f, 120, 0.9f, 0.8f, 30);
	}

	public int HitTimes = 0;

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		for (int i = 0; i < SmoothTrail.Count - 4; i += 4)
		{
			Vector2 checkTopLeft = SmoothTrail[i] * 0.5f + Projectile.Center + SmoothFallingPos.ToArray()[Math.Clamp(i, 0, SmoothFallingPos.Count - 1)] - new Vector2(80);
			var rectangle = new Rectangle((int)checkTopLeft.X, (int)checkTopLeft.Y, 160, 160);
			if (Rectangle.Intersect(rectangle, targetHitbox) != Rectangle.emptyRectangle && Projectile.timeLeft > 30)
			{
				HitTimes++;
				return true;
			}
		}
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => base.OnHitNPC(target, hit, damageDone);

	public List<Vector2> SmoothTrail = new List<Vector2>();

	public List<Vector2> SmoothFallingPos = new List<Vector2>();

	public override bool PreDraw(ref Color lightColor)
	{
		if (Projectile.timeLeft > 120)
		{
			return false;
		}
		if (OldPosSpace.Count < 3)
		{
			return false;
		}

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float value0 = (120 - Projectile.timeLeft) / 120f;
		float value1 = MathF.Pow(value0, 0.3f);
		value1 = MathF.Sin(value1 * MathF.PI);
		float width = 96f;

		var scales = new List<Vector2>();
		var SmoothTrailProjectile = new List<Vector2>();
		var smoothFallingPos = new List<Vector2>();
		for (int x = 0; x <= OldPosSpace.Count - 1; x++)
		{
			float scaleValue;
			SmoothTrailProjectile.Add(Projection2D(OldPosSpace.ToArray()[x], Vector2.zeroVector, 500, out scaleValue));
			smoothFallingPos.Add(FallingPos.ToArray()[x]);
			scales.Add(new Vector2(scaleValue, x * 40));
		}

		var scalesSmooth = new List<float>();
		List<Vector2> smoothTrail_current = GraphicsUtils.CatmullRom(SmoothTrailProjectile.ToList()); // 平滑
		List<Vector2> Smoothscales = GraphicsUtils.CatmullRom(scales.ToList()); // 平滑
		List<Vector2> SmoothFallings = GraphicsUtils.CatmullRom(smoothFallingPos.ToList());
		SmoothTrail = new List<Vector2>();
		SmoothFallingPos = new List<Vector2>();
		for (int x = 0; x < smoothTrail_current.Count; x++)
		{
			float value2 = x / (float)smoothTrail_current.Count;
			scalesSmooth.Add(Smoothscales[Math.Clamp((int)value2, 0, Smoothscales.Count - 1)].X);
			SmoothTrail.Add(smoothTrail_current[x]);
			SmoothFallingPos.Add(SmoothFallings[Math.Clamp(x, 0, SmoothFallings.Count - 1)]);
		}

		int length = SmoothTrail.Count;
		if (length <= 3)
		{
			return false;
		}

		if (!Main.gamePaused && Omega > 0.06f)
		{
		}

		var drawColor = new Color(0.9f, 0.9f, 0.9f, 0.9f);
		var bars = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; i++)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			bars.Add(SmoothTrail[i] + SmoothFallings[Math.Clamp(i, 0, SmoothFallings.Count - 1)] + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 0, 0));
			bars.Add(SmoothTrail[i] * (1f - width / 100f) + SmoothFallings[Math.Clamp(i, 0, SmoothFallings.Count - 1)] + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 1, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Melee2_black.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		FlameTrail();
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		DrawWeapon();
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		if (Projectile.timeLeft <= 70 && FallingMove.Y > 1.5f && FallingVel.Y < 1.5f)
		{
			Vector2 drawPos = Projectile.Center + FallingMove + new Vector2(260 * FirstDirection, 0) - Main.screenPosition;
			Texture2D crackTexture = Commons.ModAsset.StarSlash.Value;
			Texture2D crackTextureBlack = Commons.ModAsset.StarSlash_black.Value;
			var slashColor = new Color(0.9f, 0.75f, 0.2f, 0f) * (MathF.Pow(Projectile.timeLeft / 20f, 4) * 0.15f);
			Main.EntitySpriteDraw(crackTextureBlack, drawPos, null, Color.White, MathHelper.PiOver2, crackTextureBlack.Size() * 0.5f, new Vector2(Projectile.timeLeft / 20f, 3), SpriteEffects.None, 0);
			Main.EntitySpriteDraw(crackTexture, drawPos, null, slashColor, MathHelper.PiOver2, crackTexture.Size() * 0.5f, new Vector2(Projectile.timeLeft / 20f, 3), SpriteEffects.None, 0);
		}
		return false;
	}

	public void FlameTrail()
	{
		float width = 96f;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		var drawColor = new Color(1f, 1f, 1f, 0f);
		var bars = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; i++)
		{
			Vector2 drawPos = Projectile.Center;
			float factor = i / (SmoothTrail.Count - 1f);
			if (i == SmoothTrail.Count - 1)
			{
				factor = 0;
			}
			float w = TrailAlpha(factor);
			bars.Add(SmoothTrail[i] + SmoothFallingPos[Math.Clamp(i, 0, SmoothFallingPos.Count - 1)] + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 0, w));
			bars.Add(SmoothTrail[i] * (1f - width / 100f) + SmoothFallingPos[Math.Clamp(i, 0, SmoothFallingPos.Count - 1)] + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 1, w));
		}

		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		Effect MeleeTrail = Commons.ModAsset.MeleeTrail.Value;
		MeleeTrail.Parameters["uTransform"].SetValue(model * projection);
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Melee2.Value;
		MeleeTrail.Parameters["tex1"].SetValue(ModAsset.HeatMap_QuenchingBladeProj.Value);
		MeleeTrail.CurrentTechnique.Passes[0].Apply();
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public float TrailAlpha(float factor)
	{
		float w;
		w = MathHelper.Lerp(0f, 1, factor * 1.3f);
		return w;
	}

	public void DrawBloom()
	{
		FlameTrail();
	}

	public void DrawWeapon()
	{
		float scaleValue;
		Vector2 spaceCenter = Projection2D(OldPosSpace.ToArray()[^1], Vector2.zeroVector, 500, out scaleValue) + Projectile.Center;
		Vector2 normalize = (spaceCenter - Projectile.Center).RotatedBy(MathHelper.PiOver2) * 0.5f;
		Vector2 middleCenter = (spaceCenter + Projectile.Center) * 0.5f;
		var glowColor = Color.White;
		var offset = FallingMove - Main.screenPosition;
		var bars = new List<Vertex2D>
		{
			new Vertex2D(spaceCenter + offset, glowColor, new Vector3(1, 0, 0)),
			new Vertex2D(middleCenter + normalize + offset, glowColor, new Vector3(1, 1, 0)),
			new Vertex2D(middleCenter - normalize + offset, glowColor, new Vector3(0, 0, 0)),
			new Vertex2D(Projectile.Center + offset, glowColor, new Vector3(0, 1, 0)),
		};
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.QuenchingBladeProj.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		if (SmoothTrail.Count < 3)
		{
			return;
		}
		float width = 60f;
		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		float rotValue = 4.71f;

		var bars = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; i++)
		{
			Vector2 normal = Vector2.Normalize(SmoothTrail[i]).RotatedBy(rotValue);
			float warpValue = 1f;
			if (i < 10)
			{
				warpValue = i / 10f;
			}
			warpValue *= 0.08f;
			var drawColor0 = new Color(normal.X / 2f + 0.5f, normal.Y / 2f + 0.5f, warpValue, 1);
			var drawColor1 = new Color(normal.X / 2f + 0.5f, normal.Y / 2f + 0.5f, warpValue * 0.3f, 1);

			bars.Add(SmoothTrail[i] * 1f + SmoothFallingPos[Math.Clamp(i, 0, SmoothFallingPos.Count - 1)] + drawPos, drawColor0, new Vector3(i / (float)(SmoothTrail.Count - 1), 0, 1));
			bars.Add(SmoothTrail[i] * (1f - width / 100f) + SmoothFallingPos[Math.Clamp(i, 0, SmoothFallingPos.Count - 1)] + drawPos, drawColor1, new Vector3(i / (float)(SmoothTrail.Count - 1), 1, 1));
		}

		if (bars.Count > 3)
		{
			Main.graphics.graphicsDevice.RasterizerState = RasterizerState.CullNone;
			spriteBatch.Draw(Commons.ModAsset.Noise_flame_3.Value, bars, PrimitiveType.TriangleStrip);
		}
	}

	public static Vector3 RodriguesRotate(Vector3 origVec, Vector3 axis, float theta)
	{
		if (axis != new Vector3(0, 0, 0))
		{
			axis = Vector3.Normalize(axis);
		}
		else
		{
			axis = new Vector3(0, 0, -1);
		}
		float cos = MathF.Cos(theta);
		return cos * origVec + (1 - cos) * Vector3.Dot(origVec, axis) * axis + MathF.Sin(theta) * Vector3.Cross(origVec, axis);
	}

	public static Vector2 Projection2D(Vector3 vector, Vector2 center, float viewZ, out float scale)
	{
		float value = -viewZ / (vector.Z - viewZ);
		scale = value;
		var v = new Vector2(vector.X, vector.Y);
		return v + (value - 1) * (v - center);
	}
}
