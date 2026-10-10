using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.CorruptWormHive.Buffs;
using Everglow.Yggdrasil.CorruptWormHive.Items.Weapons;
using Everglow.Yggdrasil.CorruptWormHive.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;

namespace Everglow.Yggdrasil.CorruptWormHive.Projectiles.Melee.TrueDeathSickle;

public class TrueDeathSickle_Blade : ModProjectile, IWarpProjectile_warpStyle2, IBloomProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

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
		Projectile.extraUpdates = 3;
		Projectile.noEnchantmentVisuals = true;
		Projectile.localNPCHitCooldown = 60;
		Projectile.usesLocalNPCImmunity = true;
	}

	public bool NoSickleSelf = false;
	public int HitTimes = 0;
	public List<Vector3> OldPosSpace = new List<Vector3>();
	public Vector3 SpacePos;
	public Vector3 StartSpacePos;
	public Vector3 RotatedAxis;
	public float Omega = 0;
	public Vector2 DeltaVelocity = default;
	public List<Vector2> SmoothTrail = new List<Vector2>();
	public float TimeCounter = 0;
	public static Vector2 Offset = new Vector2(0, -10);

	public override void OnSpawn(IEntitySource source)
	{
		Player player = Main.player[Projectile.owner];
		Projectile.spriteDirection = player.direction;

		Vector2 v0 = Vector2.Normalize(Main.MouseWorld - player.MountedCenter).RotatedBy(Projectile.ai[1]);
		RotatedAxis = new Vector3(-v0.Y, v0.X, Projectile.ai[2] * Projectile.spriteDirection);
		RotatedAxis = Vector3.Normalize(RotatedAxis);
		SpacePos = GetPerpendicularUnitVector(RotatedAxis) * Projectile.ai[0];
		SpacePos = RodriguesRotate(SpacePos, RotatedAxis, -1f);
		if (player.direction == -1)
		{
			SpacePos = RodriguesRotate(SpacePos, RotatedAxis, -1.2f);
		}
		StartSpacePos = SpacePos;
		Omega = 0.27f * -player.direction;
		Projectile.ai[1] = 0;
	}

	public override bool ShouldUpdatePosition()
	{
		return false;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Projectile.Center = player.MountedCenter + Offset;
		Projectile.friendly = true;
		OldPosSpace.Add(SpacePos);
		Vector3 delta0 = SpacePos;
		SpacePos = RodriguesRotate(SpacePos, RotatedAxis, Omega * (MathF.Log(player.meleeSpeed * MathF.E) + 1));
		Projectile.ai[1] += Omega * (MathF.Log(player.meleeSpeed * MathF.E) + 1);
		delta0 = SpacePos - delta0;
		Omega *= 0.9f;
		if (Projectile.timeLeft == 114)
		{
			SoundEngine.PlaySound(new SoundStyle("Everglow/EternalResolve/Sounds/Slash").WithVolumeScale(0.33f), Projectile.Center);
		}
		DeltaVelocity = new Vector2(delta0.X, delta0.Y);
		float size;
		if (Projectile.timeLeft > 15)
		{
			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projection2D(SpacePos, Vector2.zeroVector, 500, out size).ToRotation() - MathHelper.PiOver2);
		}
		TimeCounter += player.meleeSpeed - 1;
		if (TimeCounter > 1)
		{
			TimeCounter -= 1;
			Projectile.timeLeft -= 1;
			if (Projectile.timeLeft == 114)
			{
				SoundEngine.PlaySound(new SoundStyle("Everglow/EternalResolve/Sounds/Slash").WithVolumeScale(0.33f), Projectile.Center);
			}
		}
		if (TimeCounter < -1)
		{
			TimeCounter += 1;
			Projectile.timeLeft += 1;
		}
		if (DeltaVelocity.Length() > 5)
		{
			float mulAi1 = Math.Min(MathF.Abs(Projectile.ai[1] * 0.5f), 2f);
			DevilFlame((int)(DeltaVelocity.Length() / 100 * mulAi1 + 1));
			DevilSpark((int)(DeltaVelocity.Length() / 10 * mulAi1));
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		Player player = Main.player[Projectile.owner];

		ScreenShaker playerShaker = player.GetModPlayer<ScreenShaker>();
		float ShakeStrength = 3f;
		playerShaker.FlyCamPosition = new Vector2(0, Math.Min(target.Hitbox.Width * target.Hitbox.Height / 240f * ShakeStrength, 10)).RotatedByRandom(6.283);
		modifiers.Knockback *= 5f;
		int HitType = ModContent.ProjectileType<TrueDeathSickleHit>();
		GenerateVFXFromTarget(target, 18);
		if (player.ownedProjectileCounts[HitType] < 5)
		{
			float s;
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.One, HitType, 0, 0, Projectile.owner, 15, Projection2D(SpacePos, Vector2.Zero, 500, out s).ToRotation() + Main.rand.NextFloat(-0.4f, 0.4f));
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<DeathFlame>(), 1800);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		for (int i = 0; i < SmoothTrail.Count - 4; i += 4)
		{
			var rectangle = new Rectangle((int)(SmoothTrail[i].X - 60 + Projectile.Center.X), (int)(SmoothTrail[i].Y - 60 + Projectile.Center.Y), 120, 120);
			if (Rectangle.Intersect(rectangle, targetHitbox) != Rectangle.emptyRectangle)
			{
				HitTimes++;
				return true;
			}
		}
		return false;
	}

	public override void CutTiles()
	{
		DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
		var cut = new Utils.TileActionAttempt(DelegateMethods.CutTiles);
		Vector2 beamStartPos = Projectile.Center;
		float scale;
		Vector2 beamEndPos = Projection2D(SpacePos, Vector2.zeroVector, 500, out scale) + Projectile.Center;
		Utils.PlotTileLine(beamStartPos, beamEndPos, Projectile.width * Projectile.scale, cut);
	}

	public override void OnKill(int timeLeft)
	{
		foreach (Projectile projectile in Main.projectile)
		{
			if (projectile.active && projectile.type == Type)
			{
				if (projectile.timeLeft > Projectile.timeLeft)
				{
					return;
				}
			}
		}
		var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TrueDeathSickle_SickleBack>(), 0, 0, Projectile.owner, Projectile.ai[0], Projectile.ai[1]);
		proj.spriteDirection = Projectile.spriteDirection;
		var tDSSB = proj.ModProjectile as TrueDeathSickle_SickleBack;
		tDSSB.SpacePos = SpacePos;
		tDSSB.RotatedAxis = RotatedAxis;
	}

	public void GenerateVFXFromTarget(NPC target, int frequency)
	{
		for (int g = 0; g < frequency; g++)
		{
			var df = new DevilFlame3DSickle_worldCoordDust
			{
				Velocity3D = new Vector3(new Vector2(0, Main.rand.NextFloat(6, 9f)).RotatedByRandom(MathHelper.TwoPi), 0),
				Active = true,
				Visible = true,
				Position3D = new Vector3(target.Center, 0),
				rotateAxis = new Vector3(0, 0, 1),
				Scale = Main.rand.NextFloat(6, 12),
				MaxTime = Main.rand.Next(36, 40),
				OwnerWhoAmI = Projectile.owner,
				ai = new float[] { Main.rand.NextFloat(0, 1f), Main.rand.NextFloat(-0.1f, 0.1f), 0f },
			};
			Ins.VFXManager.Add(df);
		}
	}

	public void DevilFlame(int times = 1)
	{
		for (int g = 0; g < times; g++)
		{
			var df = new DevilFlame3DSickleDust
			{
				Velocity3D = Vector3.Normalize(SpacePos - OldPosSpace[OldPosSpace.Count - 1]) * 15f,
				Active = true,
				Visible = true,
				Position3D = Vector3.Lerp(OldPosSpace[OldPosSpace.Count - 1], SpacePos, Main.rand.NextFloat(0, 1f)),
				rotateAxis = RotatedAxis,
				Scale = Main.rand.NextFloat(12, 26),
				MaxTime = Main.rand.Next(36, 40),
				OwnerWhoAmI = Projectile.owner,
				ai = new float[] { Main.rand.NextFloat(0, 1f), Main.rand.NextFloat(0, 0.1f) * -Projectile.spriteDirection, 0f },
			};
			Ins.VFXManager.Add(df);
		}
	}

	public void DevilSpark(int times = 1)
	{
		for (int g = 0; g < times; g++)
		{
			var df = new DevilSpark3DSickleDust
			{
				Velocity3D = Vector3.Normalize(SpacePos - OldPosSpace[OldPosSpace.Count - 1]) * Main.rand.NextFloat(5f, 24f),
				Active = true,
				Visible = true,
				Position3D = Vector3.Lerp(OldPosSpace[OldPosSpace.Count - 1], SpacePos, Main.rand.NextFloat(0, 1f)),
				rotateAxis = RotatedAxis,
				Scale = Main.rand.NextFloat(1, 6),
				MaxTime = Main.rand.Next(6, 70),
				OwnerWhoAmI = Projectile.owner,
				ai = new float[] { Main.rand.NextFloat(0, 1f), Main.rand.NextFloat(0.01f, 0.1f) * -Projectile.spriteDirection, Main.rand.NextFloat(4f, 12f) },
			};
			Ins.VFXManager.Add(df);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
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
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		float value0 = (120 - Projectile.timeLeft) / 120f;
		float value1 = MathF.Pow(value0, 0.3f);
		value1 = MathF.Sin(value1 * MathF.PI);
		float width = value1 * 100f;
		DrawSickle(0.5f, -Omega * 20);
		DrawSickle(0.25f, -Omega * 50);

		var scales = new List<Vector2>();
		var SmoothTrailProjectile = new List<Vector2>();
		for (int x = 0; x <= OldPosSpace.Count - 1; x++)
		{
			float scaleValue;
			SmoothTrailProjectile.Add(Projection2D(OldPosSpace[x], Vector2.zeroVector, 500, out scaleValue));
			scales.Add(new Vector2(scaleValue, x * 40));
		}

		var scalesSmooth = new List<float>();
		List<Vector2> smoothTrail_current = GraphicsUtils.CatmullRom(SmoothTrailProjectile.ToList()); // 平滑
		List<Vector2> Smoothscales = GraphicsUtils.CatmullRom(scales.ToList()); // 平滑
		SmoothTrail = new List<Vector2>();
		for (int x = 0; x < smoothTrail_current.Count; x++)
		{
			float value2 = x / (float)smoothTrail_current.Count;
			scalesSmooth.Add(Smoothscales[Math.Clamp((int)value2, 0, Smoothscales.Count - 1)].X);
			SmoothTrail.Add(smoothTrail_current[x]);
		}

		int length = SmoothTrail.Count;
		if (length <= 3)
		{
			return false;
		}
		float flameSize = 24f;
		float flameTimer = -(float)Main.time * 0.06f;

		// dark background color.
		Color drawColor = Color.White;
		var bars = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; i++)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			float colorSize = 1f;
			if (i >= SmoothTrail.Count - 15)
			{
				colorSize *= (SmoothTrail.Count - i - 1) / 15f;
			}
			if (i < 24)
			{
				colorSize *= i / 24f;
			}
			bars.Add(SmoothTrail[i] + drawPos, drawColor * colorSize, new Vector3(i / flameSize + flameTimer, 0.5f, 0));
			bars.Add(SmoothTrail[i] * (1f - width / 100f) + drawPos, drawColor * colorSize, new Vector3(i / flameSize * 1.3f + flameTimer, 0f, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0_black.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		// flame color
		drawColor = new Color(0.25f, 0, 0.13f, 0) * value1;
		bars = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; i++)
		{
			var newColor = Color.Lerp(new Color(0.1f, 0.8f, 1f, 0), drawColor, MathF.Pow(1 - i / (float)SmoothTrail.Count, 0.6f));
			Vector2 drawPos = Projectile.Center - Main.screenPosition;

			float colorSize = 1f;
			if (i >= SmoothTrail.Count - 15)
			{
				colorSize *= (SmoothTrail.Count - i - 1) / 15f;
			}
			if (i < 24)
			{
				colorSize *= i / 24f;
			}
			bars.Add(SmoothTrail[i] + drawPos, newColor * colorSize, new Vector3(i / flameSize + flameTimer, 0.5f, 0));
			bars.Add(SmoothTrail[i] * (1f - width / 100f) + drawPos, newColor * colorSize, new Vector3(i / flameSize * 1.3f + flameTimer, 0f, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		// add flame color
		if (value1 > 0.5f)
		{
			drawColor = new Color(1.6f * lightColor.R / 255f, 0.8f * lightColor.G / 255f, 3f * lightColor.B / 255f, 0) * (value1 - 0.5f) * 8;
			bars = new List<Vertex2D>();
			for (int i = 0; i < SmoothTrail.Count; i++)
			{
				Vector2 drawPos = Projectile.Center - Main.screenPosition;
				float colorSize = 1f;
				if (i >= SmoothTrail.Count - 15)
				{
					colorSize *= (SmoothTrail.Count - i - 1) / 15f;
				}
				if (i < 24)
				{
					colorSize *= i / 24f;
				}
				bars.Add(SmoothTrail[i] + drawPos, drawColor * colorSize, new Vector3(0.5f, i / flameSize, 0));
				bars.Add(SmoothTrail[i] * (1f - width / 100f) + drawPos, drawColor * colorSize, new Vector3(0.24f, i / flameSize, 0));
			}
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.StarSlash.Value;
			if (bars.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}

		// rim of blade trade.
		drawColor = new Color(0.6f, 1f, 1f, 0);
		bars = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; i++)
		{
			var newColor = Color.Lerp(drawColor, new Color(0.3f, 0.0f, 0.1f, 0), i / (float)SmoothTrail.Count);
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			bars.Add(SmoothTrail[i] + new Vector2(0, -4) + drawPos, newColor, new Vector3(0.56f, i / (float)(SmoothTrail.Count - 1), 0));
			bars.Add(SmoothTrail[i] + new Vector2(0, 4) + drawPos, newColor, new Vector3(0.44f, i / (float)(SmoothTrail.Count - 1), 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_0.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		DrawSickle();
		DrawSickle(1, 0, true);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawSickle(float fade = 1, float addRotation = 0, bool glowMask = false)
	{
		if (NoSickleSelf)
		{
			return;
		}
		var bars = new List<Vertex2D>();
		float scaleValue;

		// Get the normal of rotated axis and Z axis.
		var normalOfRotatedAxisAndZ = Vector3.Normalize(Vector3.Cross(new Vector3(0, 0, -1), RotatedAxis));
		float angleOfZAndRotatedAxis = GetAngleBetweenVectors(new Vector3(0, 0, -1), RotatedAxis);

		// Texture coords
		var texCoordsOrig = new List<Vector3>
		{
			new Vector3(155, 58, 0),
			new Vector3(42, 174, 0),
			new Vector3(6, 174, 0),
			new Vector3(7, 13, 0),
			new Vector3(127, 15, 0),
		};

		// Roteted texcoords to get space coords.
		// Then rotated the space coords to sickel rotation.
		float sickleRot = Projectile.ai[1];

		// GetAngleBetweenVectors(StartSpacePos, SpacePos);
		sickleRot += MathHelper.Pi / 5f - MathHelper.Pi / 3f * Projectile.spriteDirection;
		sickleRot += addRotation;

		// This is a magic value that coorded in the handle, the rotated center of the sickle.
		var unrotatedCoord1 = new Vector3(34, 132, 0);
		if (Projectile.spriteDirection == 1)
		{
			(unrotatedCoord1.Y, unrotatedCoord1.X) = (unrotatedCoord1.X, unrotatedCoord1.Y);
		}
		var texCoordsRotated = new List<Vector3>();
		for (int i = 0; i < texCoordsOrig.Count; i++)
		{
			Vector3 unrotatedCoord = texCoordsOrig[i];
			if (Projectile.spriteDirection == 1)
			{
				(unrotatedCoord.Y, unrotatedCoord.X) = (unrotatedCoord.X, unrotatedCoord.Y);
			}
			Vector3 firstSpaceCoord = RodriguesRotate(unrotatedCoord, normalOfRotatedAxisAndZ, -angleOfZAndRotatedAxis);
			firstSpaceCoord -= RodriguesRotate(unrotatedCoord1, normalOfRotatedAxisAndZ, -angleOfZAndRotatedAxis);
			firstSpaceCoord.Y += 8;
			texCoordsRotated.Add(RodriguesRotate(firstSpaceCoord, RotatedAxis, sickleRot));
		}
		var texCoordsProjected = new List<Vector2>();
		for (int i = 0; i < texCoordsRotated.Count; i++)
		{
			// Set the sickle size by ai[0].
			texCoordsProjected.Add(Projection2D(texCoordsRotated[i], Vector2.zeroVector, 500, out scaleValue) * Projectile.ai[0] / 130f/* + sickleTip - projectedSickleTip*/);
		}
		float size = 174f;
		if (!glowMask)
		{
			AddVertexWCS(bars, texCoordsProjected[0] + Projectile.Center, texCoordsOrig[0] / size, fade);
			AddVertexWCS(bars, texCoordsProjected[1] + Projectile.Center, texCoordsOrig[1] / size, fade);
			AddVertexWCS(bars, texCoordsProjected[4] + Projectile.Center, texCoordsOrig[4] / size, fade);

			AddVertexWCS(bars, texCoordsProjected[4] + Projectile.Center, texCoordsOrig[4] / size, fade);
			AddVertexWCS(bars, texCoordsProjected[1] + Projectile.Center, texCoordsOrig[1] / size, fade);
			AddVertexWCS(bars, texCoordsProjected[3] + Projectile.Center, texCoordsOrig[3] / size, fade);

			AddVertexWCS(bars, texCoordsProjected[3] + Projectile.Center, texCoordsOrig[3] / size, fade);
			AddVertexWCS(bars, texCoordsProjected[1] + Projectile.Center, texCoordsOrig[1] / size, fade);
			AddVertexWCS(bars, texCoordsProjected[2] + Projectile.Center, texCoordsOrig[2] / size, fade);
		}
		else
		{
			Color color = new Color(1f, 1f, 1f, 0) * fade;
			Vector2 offset = Projectile.Center - Main.screenPosition;
			bars.Add(texCoordsProjected[0] + offset, color, texCoordsOrig[0] / size);
			bars.Add(texCoordsProjected[1] + offset, color, texCoordsOrig[1] / size);
			bars.Add(texCoordsProjected[4] + offset, color, texCoordsOrig[4] / size);

			bars.Add(texCoordsProjected[4] + offset, color, texCoordsOrig[4] / size);
			bars.Add(texCoordsProjected[1] + offset, color, texCoordsOrig[1] / size);
			bars.Add(texCoordsProjected[3] + offset, color, texCoordsOrig[3] / size);

			bars.Add(texCoordsProjected[3] + offset, color, texCoordsOrig[3] / size);
			bars.Add(texCoordsProjected[1] + offset, color, texCoordsOrig[1] / size);
			bars.Add(texCoordsProjected[2] + offset, color, texCoordsOrig[2] / size);
		}

		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.TrueDeathSickle_proj.Value;
		if (glowMask)
		{
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.TrueDeathSickle_proj_glow.Value;
		}
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);
		}
	}

	public void DrawBloom()
	{
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		if (SmoothTrail.Count < 3)
		{
			return;
		}
		float value0 = (120 - Projectile.timeLeft) / 120f;
		float value1 = MathF.Pow(value0, 0.3f);
		value1 = MathF.Sin(value1 * MathF.PI);
		float width = value1 * 60f;
		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		float rotValue = 4.71f;

		var bars = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; i++)
		{
			Vector2 normal = Vector2.Normalize(SmoothTrail[i]).RotatedBy(rotValue);
			var drawColor0 = new Color(normal.X / 2f + 0.5f, normal.Y / 2f + 0.5f, 1f, 0);
			var drawColor1 = new Color(normal.X / 2f + 0.5f, normal.Y / 2f + 0.5f, 0, 0);

			bars.Add(SmoothTrail[i] + drawPos, drawColor0, new Vector3(0, 1 - i / (float)(SmoothTrail.Count - 1), 1));
			bars.Add(SmoothTrail[i] * (1f - width / 100f) + drawPos, drawColor1, new Vector3(1f, i / (float)(SmoothTrail.Count - 1), 1));
		}

		if (bars.Count > 3)
		{
			Main.graphics.graphicsDevice.RasterizerState = RasterizerState.CullNone;
			spriteBatch.Draw(Commons.ModAsset.Melee.Value, bars, PrimitiveType.TriangleStrip);
		}
	}

	public static float GetAngleBetweenVectors(Vector3 v1, Vector3 v2)
	{
		// 计算点积
		float dotProduct = Vector3.Dot(v1, v2);

		// 计算向量的长度
		float lengthV1 = v1.Length();
		float lengthV2 = v2.Length();

		// 计算夹角（弧度制）
		float angleInRadians = (float)Math.Acos(dotProduct / (lengthV1 * lengthV2));

		return angleInRadians;
	}

	public static void AddVertexWCS(List<Vertex2D> bars, Vector2 position, Vector3 texCoord, float fade = 1f)
	{
		bars.Add(position - Main.screenPosition, Lighting.GetColor(position.ToTileCoordinates()) * fade, texCoord);
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

	public static Vector3 GetPerpendicularUnitVector(Vector3 v1)
	{
		// 计算任意一个不平行的向量
		var arbitraryVector = Vector3.Cross(v1, new Vector3(1, 0, 0));

		// 如果 arbitraryVector 是零向量，说明 v1 与 (1,0,0) 平行，我们需要选择一个不同的向量
		if (arbitraryVector.LengthSquared() == 0)
		{
			arbitraryVector = Vector3.Cross(v1, new Vector3(0, 1, 0));
		}

		// 计算垂直向量并归一化
		var perpendicularVector = Vector3.Cross(v1, arbitraryVector);
		perpendicularVector.Normalize();

		return perpendicularVector;
	}
}
