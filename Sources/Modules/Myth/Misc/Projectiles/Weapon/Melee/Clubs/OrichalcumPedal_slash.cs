using Everglow.Commons.DataStructures;
using Everglow.Myth.Misc.Dusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.Clubs;

public class OrichalcumPedal_slash : ModProjectile, IWarpProjectile_warpStyle2
{
	public override string Texture => Commons.ModAsset.StabbingProjectile_Mod;

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
		Vector2 v0 = new Vector2(0, Main.rand.NextFloat(2, 3)).RotatedByRandom(6.283);
		RotatedAxis = new Vector3(v0.X, -8, v0.Y);
		v0 = new Vector2(0, Main.rand.NextFloat(90, 150)).RotatedByRandom(6.283) * Projectile.ai[0];
		SpacePos = new Vector3(v0.X, Main.rand.NextFloat(-5, 5), v0.Y);
		SpacePos = RodriguesRotate(SpacePos, RotatedAxis, Main.rand.NextFloat(6.283f));
		Omega = 0.7f;
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
		Projectile.friendly = true;
		OldPosSpace.Add(SpacePos);
		Vector3 delta0 = SpacePos;
		SpacePos = RodriguesRotate(SpacePos, RotatedAxis, Omega);
		delta0 = SpacePos - delta0;
		Omega *= 0.9f;
		if (Projectile.timeLeft == 114)
		{
			SoundEngine.PlaySound(new SoundStyle("Everglow/EternalResolve/Sounds/Slash").WithVolumeScale(0.33f), Projectile.Center);
		}
		if (Main.rand.NextBool(8) && Omega > 0.4f)
		{
			Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(SpacePos.X, SpacePos.Y), new Vector2(delta0.X, delta0.Y) * 1.5f, ModContent.ProjectileType<OrichalcumPedal>(), Projectile.damage / 8, 0, Projectile.owner, Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.Next(3));
			p0.scale = Main.rand.NextFloat(0.8f, 1.2f) * Projectile.ai[0];
		}
		DeltaVelocity = new Vector2(delta0.X, delta0.Y);
	}

	public int HitTimes = 0;

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		for (int i = 0; i < SmoothTrail.Count - 4; i += 4)
		{
			Rectangle rectangle = new Rectangle((int)(SmoothTrail[i].X - 10 + Projectile.Center.X), (int)(SmoothTrail[i].Y - 10 + Projectile.Center.Y), 20, 20);
			if (Rectangle.Intersect(rectangle, targetHitbox) != Rectangle.emptyRectangle)
			{
				Projectile.damage /= 2;
				HitTimes++;
				if (HitTimes > 3)
				{
					Projectile.friendly = false;
				}
				return true;
			}
		}
		return false;
	}

	public List<Vector2> SmoothTrail = new List<Vector2>();

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		if (Projectile.timeLeft > 120)
		{
			return false;
		}
		if (OldPosSpace.Count < 3)
		{
			return false;
		}

		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float value0 = (120 - Projectile.timeLeft) / 120f;
		float value1 = MathF.Pow(value0, 0.3f);
		value1 = MathF.Sin(value1 * MathF.PI);
		float width = value1 * 70f;

		List<Vector2> scales = new List<Vector2>();
		List<Vector2> SmoothTrailProjectile = new List<Vector2>();
		for (int x = 0; x <= OldPosSpace.Count - 1; x++)
		{
			float scaleValue;
			SmoothTrailProjectile.Add(Projection2D(OldPosSpace[x], Vector2.zeroVector, 500, out scaleValue));
			scales.Add(new Vector2(scaleValue, x * 40));
		}

		List<float> scalesSmooth = new List<float>();
		List<Vector2> SmoothTrailX = GraphicsUtils.CatmullRom(SmoothTrailProjectile.ToList()); // 平滑
		List<Vector2> Smoothscales = GraphicsUtils.CatmullRom(scales.ToList()); // 平滑
		SmoothTrail = new List<Vector2>();
		for (int x = 0; x < SmoothTrailX.Count; x++)
		{
			float value2 = x / (float)SmoothTrailX.Count;
			scalesSmooth.Add(Smoothscales[Math.Clamp((int)value2, 0, Smoothscales.Count - 1)].X);
			SmoothTrail.Add(SmoothTrailX[x]);
		}

		int length = SmoothTrail.Count;
		if (length <= 3)
		{
			return false;
		}

		if (!Main.gamePaused && Omega > 0.06f)
		{
			for (int i = 0; i < (120 - Projectile.timeLeft) / 15f * Projectile.ai[0]; i++)
			{
				int index = Main.rand.Next(1, SmoothTrail.Count);
				float range = Main.rand.NextFloat(0.6f, 0.96f);
				Dust dust = Dust.NewDustDirect(Projectile.Center + SmoothTrail[index] * range - new Vector2(4), 0, 0, ModContent.DustType<OrichalcunPetals_dust>());
				dust.velocity = (SmoothTrail[index] - SmoothTrail[index - 1]) * 0.75f * range;
				dust.scale = Main.rand.NextFloat(0.85f, 1.15f);
			}
		}

		Color drawColor = new Color(0.9f, 0.9f, 0.9f, 0.9f) * 0.5f;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; i++)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			bars.Add(SmoothTrail[i] + drawPos, drawColor, new Vector3(0.5f, i / (float)(SmoothTrail.Count - 1), 0));
			bars.Add(SmoothTrail[i] * (1f - width / 100f) + drawPos, drawColor, new Vector3(0.44f, i / (float)(SmoothTrail.Count - 1), 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.StarSlash_black.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		drawColor = new Color(1f * lightColor.R / 255f, 0.2f * lightColor.G / 255f * value1, 1.5f * lightColor.B / 255f * value1, 0) * value1;
		bars = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; i++)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			bars.Add(SmoothTrail[i] + drawPos, drawColor, new Vector3(0.5f, i / (float)(SmoothTrail.Count - 1), 0));
			bars.Add(SmoothTrail[i] * (1f - width / 100f) + drawPos, drawColor, new Vector3(0.44f, i / (float)(SmoothTrail.Count - 1), 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.StarSlash.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		if (value1 > 0.5f)
		{
			drawColor = new Color(1.6f * lightColor.R / 255f, 0.8f * lightColor.G / 255f, 3f * lightColor.B / 255f, 0) * (value1 - 0.5f) * 8;
			bars = new List<Vertex2D>();
			for (int i = 0; i < SmoothTrail.Count; i++)
			{
				Vector2 drawPos = Projectile.Center - Main.screenPosition;
				bars.Add(SmoothTrail[i] + drawPos, drawColor, new Vector3(0.5f, i / (float)(SmoothTrail.Count - 1), 0));
				bars.Add(SmoothTrail[i] * (1f - width / 100f) + drawPos, drawColor, new Vector3(0.24f, i / (float)(SmoothTrail.Count - 1), 0));
			}
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.StarSlash.Value;
			if (bars.Count > 3)
			{
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
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

		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i < SmoothTrail.Count; i++)
		{
			Vector2 normal = Vector2.Normalize(SmoothTrail[i]).RotatedBy(rotValue);
			Color drawColor0 = new Color(normal.X / 2f + 0.5f, normal.Y / 2f + 0.5f, 0.8f, 0);
			Color drawColor1 = new Color(normal.X / 2f + 0.5f, normal.Y / 2f + 0.5f, 0, 0);

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
