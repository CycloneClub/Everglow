using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities.BuffHelpers;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_TaijutsuSlash_Down : ModProjectile, IWarpProjectile_warpStyle2
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.TownNPCProjectiles;

	public override string Texture => ModAsset.FevensTaijutsu_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 120;
		Projectile.extraUpdates = 3;

		Projectile.localNPCHitCooldown = 60;
		Projectile.usesLocalNPCImmunity = true;

		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 180000;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public Queue<Vector3> OldPosSpace = new Queue<Vector3>();
	public Queue<Vector2> FallingPos = new Queue<Vector2>();
	public Vector2 FallingMove;
	public Vector2 FallingVel;
	public Vector3 SpacePos;
	public Vector3 RotatedAxis;
	public float Omega = 0;
	public Vector2 DeltaVelocity = default;
	public List<int> HitPlayers = new List<int>();
	public int FirstDirection = 0;

	public override void OnSpawn(IEntitySource source)
	{
		RotatedAxis = new Vector3(0, 0, 5);
		Vector2 v0 = new Vector2(0, -270) * Projectile.ai[0];
		SpacePos = new Vector3(v0, 0);
		SpacePos = RodriguesRotate(SpacePos, RotatedAxis, Projectile.ai[1]);
		FallingMove = Vector2.zeroVector;
		FallingVel = Vector2.zeroVector;
		Omega = -0.45f;
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
		if (FirstDirection == 0)
		{
			FirstDirection = Projectile.spriteDirection;
		}
		OldPosSpace.Enqueue(SpacePos);
		FallingPos.Enqueue(FallingMove);
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

			if (!Collision.SolidCollision(Projectile.Center + FallingMove - new Vector2(30), 60, 60))
			{
				FallingVel.Y += 1.5f;
			}
			else
			{
				if (FallingVel.Y > 20)
				{
					SmashEffect();
				}
				FallingVel.Y *= 0;
			}
		}
		delta0 = SpacePos - delta0;
		Omega *= 0.9f;
		if (Projectile.timeLeft == 114)
		{
			Projectile.hostile = true;

			// SoundEngine.PlaySound(new SoundStyle("Everglow/EternalResolve/Sounds/Slash").WithVolumeScale(0.33f), Projectile.Center);
		}
		DeltaVelocity = new Vector2(delta0.X, delta0.Y);
	}

	public void SmashEffect()
	{
		ShakerManager.AddShaker(Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.Pi), 120, 30f, 200, 0.9f, 0.8f, 150);
		for (int g = 0; g < 24; g++)
		{
			Vector2 newVelocity = new Vector2(0, -(Main.rand.NextFloat(25f, 52f) + g * 8)).RotatedBy(Main.rand.NextFloat(-0.04f, 0.14f) + g / 24f * FirstDirection);
			Vector2 pos = Projectile.Center + FallingMove + new Vector2((60 + 12 * g) * FirstDirection, 3) - newVelocity * 1;
			var somg = new Fevens_TaijutsuSmash
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = pos,
				MaxTime = Main.rand.Next(25, 68),
				Scale = Main.rand.NextFloat(30f, 80f),
				ai = new float[] { 0, 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		Projectile.timeLeft = 70;
		Projectile.extraUpdates = 2;
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
				Projectile.damage /= 2;
				HitTimes++;
				return true;
			}
		}
		return false;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		if (!HitPlayers.Contains(target.whoAmI))
		{
			HitPlayers.Add(target.whoAmI);
		}
		target.AddBuff(ModContent.BuffType<ShortImmune12>(), 15);
		base.OnHitPlayer(target, info);
	}

	public List<Vector2> SmoothTrail = new List<Vector2>();

	public List<Vector2> SmoothFallingPos = new List<Vector2>();

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
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Melee_Black.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		drawColor = new Color(0.3f, 0.1f, 0.9f, 0f) * value1;
		bars = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; i++)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			bars.Add(SmoothTrail[i] + SmoothFallings[Math.Clamp(i, 0, SmoothFallings.Count - 1)] + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 0, 0));
			bars.Add(SmoothTrail[i] * (1f - width / 100f) + SmoothFallings[Math.Clamp(i, 0, SmoothFallings.Count - 1)] + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 1, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Melee.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		if (value1 > 0.5f)
		{
			drawColor = new Color(0.1f, 0.1f, 0.9f, 0f) * (value1 - 0.5f) * 8;
			bars = new List<Vertex2D>();
			for (int i = 0; i < SmoothTrail.Count; i++)
			{
				Vector2 drawPos = Projectile.Center - Main.screenPosition;
				Lighting.AddLight(SmoothTrail[i] + drawPos + Main.screenPosition, new Vector3(0.4f, 0.1f, 0.9f) * (value1 - 0.5f));
				bars.Add(SmoothTrail[i] + SmoothFallings[Math.Clamp(i, 0, SmoothFallings.Count - 1)] + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 0, 0));
				bars.Add(SmoothTrail[i] * (1f - width / 100f) + SmoothFallings[Math.Clamp(i, 0, SmoothFallings.Count - 1)] + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 1, 0));
			}
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Melee.Value;
			if (bars.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}
		DrawWeapon();
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		if (Projectile.timeLeft <= 70 && FallingMove.Y > 1.5f && FallingVel.Y < 1.5f)
		{
			Vector2 drawPos = Projectile.Center + FallingMove + new Vector2(260 * FirstDirection, 0) - Main.screenPosition;
			Texture2D crackTexture = Commons.ModAsset.StarSlash.Value;
			Texture2D crackTextureBlack = Commons.ModAsset.StarSlash_black.Value;
			var slashColor = new Color(0.4f, 0.15f, 0.6f, 0f) * (MathF.Pow(Projectile.timeLeft / 20f, 4) * 0.15f);
			Main.EntitySpriteDraw(crackTextureBlack, drawPos, null, Color.White, MathHelper.PiOver2, crackTextureBlack.Size() * 0.5f, new Vector2(Projectile.timeLeft / 20f, 3), SpriteEffects.None, 0);
			Main.EntitySpriteDraw(crackTexture, drawPos, null, slashColor, MathHelper.PiOver2, crackTexture.Size() * 0.5f, new Vector2(Projectile.timeLeft / 20f, 3), SpriteEffects.None, 0);
		}
		return false;
	}

	public void DrawWeapon()
	{
		float scaleValue;
		Vector2 spaceCenter = Projection2D(OldPosSpace.ToArray()[^1], Vector2.zeroVector, 500, out scaleValue) + Projectile.Center;
		Vector2 normalize = (spaceCenter - Projectile.Center).RotatedBy(MathHelper.PiOver2) * 0.5f;
		Vector2 middleCenter = (spaceCenter + Projectile.Center) * 0.5f;
		var glowColor = Color.White;
		if (Omega < 0.1f)
		{
			glowColor *= Omega / 0.1f;
		}
		var bars = new List<Vertex2D>
		{
			new Vertex2D(spaceCenter - Main.screenPosition, glowColor, new Vector3(1, 0, 0)),
			new Vertex2D(middleCenter + normalize - Main.screenPosition, glowColor, new Vector3(1, 1, 0)),
			new Vertex2D(middleCenter - normalize - Main.screenPosition, glowColor, new Vector3(0, 0, 0)),
			new Vertex2D(Projectile.Center - Main.screenPosition, glowColor, new Vector3(0, 1, 0)),
		};
		Main.graphics.GraphicsDevice.Textures[0] = ModAsset.FevensTaijutsu.Value;
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
		float value0 = (120 - Projectile.timeLeft) / 120f;
		float value1 = MathF.Pow(value0, 0.3f);
		value1 = MathF.Sin(value1 * MathF.PI);
		float width = value1 * 20f;
		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		float rotValue = 4.71f;

		var bars = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; i++)
		{
			Vector2 normal = Vector2.Normalize(SmoothTrail[i]).RotatedBy(rotValue);
			var drawColor0 = new Color(normal.X / 2f + 0.5f, normal.Y / 2f + 0.5f, 0.8f, 0);
			var drawColor1 = new Color(normal.X / 2f + 0.5f, normal.Y / 2f + 0.5f, 0, 0);

			bars.Add(SmoothTrail[i] + drawPos, drawColor0, new Vector3(0.5f, i / (float)(SmoothTrail.Count - 1), 1));
			bars.Add(SmoothTrail[i] * (1f - width / 100f) + drawPos, drawColor1, new Vector3(0.44f, i / (float)(SmoothTrail.Count - 1), 1));
		}

		if (bars.Count > 3)
		{
			Main.graphics.graphicsDevice.RasterizerState = RasterizerState.CullNone;
			spriteBatch.Draw(Commons.ModAsset.StarSlash.Value, bars, PrimitiveType.TriangleStrip);
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
