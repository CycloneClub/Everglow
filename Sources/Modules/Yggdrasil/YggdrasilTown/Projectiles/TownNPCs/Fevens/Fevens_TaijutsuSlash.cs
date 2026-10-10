using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities.BuffHelpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;

public class Fevens_TaijutsuSlash : ModProjectile, IWarpProjectile_warpStyle2
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
	}

	public List<Vector3> OldPosSpace = new List<Vector3>();
	public Vector3 SpacePos;
	public Vector3 RotatedAxis;
	public float Omega = 0;
	public Vector2 DeltaVelocity = default;

	public override void OnSpawn(IEntitySource source)
	{
		RotatedAxis = new Vector3(0, 0, 5);
		Vector2 v0 = new Vector2(0, -270) * Projectile.ai[0];
		SpacePos = new Vector3(v0, 0);
		SpacePos = RodriguesRotate(SpacePos, RotatedAxis, Projectile.ai[1]);
		Omega = Projectile.ai[2];
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
		return false;
	}

	public override void AI()
	{
		OldPosSpace.Add(SpacePos);
		Vector3 delta0 = SpacePos;
		if (Projectile.direction == -1)
		{
			Omega *= Projectile.direction;
			Projectile.direction = 1;
		}
		SpacePos = RodriguesRotate(SpacePos, RotatedAxis, Omega);
		delta0 = SpacePos - delta0;
		Omega *= 0.9f;
		if (Projectile.timeLeft == 114)
		{
			Projectile.hostile = true;

			// SoundEngine.PlaySound(new SoundStyle("Everglow/EternalResolve/Sounds/Slash").WithVolumeScale(0.33f), Projectile.Center);
		}
		DeltaVelocity = new Vector2(delta0.X, delta0.Y);
	}

	public int HitTimes = 0;

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		for (int i = 0; i < SmoothTrail.Count - 4; i += 4)
		{
			var rectangle = new Rectangle((int)(SmoothTrail[i].X * 0.5f - 80 + Projectile.Center.X), (int)(SmoothTrail[i].Y * 0.5f - 80 + Projectile.Center.Y), 160, 160);
			if (Rectangle.Intersect(rectangle, targetHitbox) != Rectangle.emptyRectangle && Projectile.timeLeft > 30)
			{
				Projectile.damage /= 2;
				HitTimes++;
				if (HitTimes > 3)
				{
					Projectile.hostile = false;
				}
				return true;
			}
		}
		return false;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		target.AddBuff(ModContent.BuffType<ShortImmune12>(), 15);
		base.OnHitPlayer(target, info);
	}

	public List<Vector2> SmoothTrail = new List<Vector2>();

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

		if (!Main.gamePaused && Omega > 0.06f)
		{
		}

		var drawColor = new Color(0.9f, 0.9f, 0.9f, 0.9f);
		var bars = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; i++)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			bars.Add(SmoothTrail[i] + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 0, 0));
			bars.Add(SmoothTrail[i] * (1f - width / 100f) + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 1, 0));
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
			bars.Add(SmoothTrail[i] + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 0, 0));
			bars.Add(SmoothTrail[i] * (1f - width / 100f) + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 1, 0));
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
				bars.Add(SmoothTrail[i] + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 0, 0));
				bars.Add(SmoothTrail[i] * (1f - width / 100f) + drawPos, drawColor, new Vector3(i / (float)(SmoothTrail.Count - 1), 1, 0));
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
		return false;
	}

	public void DrawWeapon()
	{
		float scaleValue = 1f;
		Vector2 spaceCenter = Projection2D(OldPosSpace[^1], Vector2.zeroVector, 500, out scaleValue) + Projectile.Center;
		Vector2 normalize = (spaceCenter - Projectile.Center).RotatedBy(MathHelper.PiOver2) * 0.5f;
		Vector2 middleCenter = (spaceCenter + Projectile.Center) * 0.5f;
		var lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
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
