using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.CityOfMagicFlute.VFXs;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.IstafelsEffects;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class IstafelsSunfireGrasp_Explosion : ModProjectile, IWarpProjectile_warpStyle2
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => ModAsset.TerraViewerHowitzer_grenade_fall_Mod;

	public int Timer = 0;

	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 120;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 0;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.DamageType = DamageClass.Magic;
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 30000;
		Timer = 0;
	}

	public override void AI()
	{
		Timer++;
		if (Timer < 20 && Timer % 5 == 0)
		{
			LargeFlame(60);
			SmallFlame(30);
		}
		base.AI();
	}

	public void LargeFlame(int count)
	{
		for (int x = 0; x < count; x++)
		{
			Vector2 newVelocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 12f).RotatedByRandom(MathHelper.TwoPi);
			var somg = new MissleFlameDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(6), 0).RotatedByRandom(6.283),
				maxTime = Main.rand.Next(90, 120),
				scale = Main.rand.NextFloat(24f, 36f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public void SmallFlame(int count)
	{
		for (int x = 0; x < count; x++)
		{
			Vector2 newVelocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 8f).RotatedByRandom(MathHelper.TwoPi);
			var somg = new MissleFlameDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(4), 0).RotatedByRandom(6.283),
				maxTime = Main.rand.Next(50, 70),
				scale = Main.rand.NextFloat(12f, 20f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.ai[2] = 600;
		Projectile.velocity *= 0;
		LargeFlame(60);
		SmallFlame(30);
		for (int g = 0; g < 40; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(2, 7)).RotatedByRandom(MathHelper.TwoPi);
			float mulScale = Main.rand.NextFloat(12f, 24f);
			var drop = new IstafelsSunfireDrop
			{
				velocity = afterVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(20f), 0).RotatedByRandom(6.283),
				maxTime = Main.rand.Next(112, 144),
				scale = mulScale,
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(drop);
		}
		for (int g = 0; g < 200; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(2, 24)).RotatedByRandom(MathHelper.TwoPi);
			float mulScale = Main.rand.NextFloat(4f, 8f);
			var drop = new IstafelsSunfireDrop
			{
				velocity = afterVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(20f), 0).RotatedByRandom(6.283),
				maxTime = Main.rand.Next(32, 124),
				scale = mulScale,
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(drop);
		}
		for (int g = 0; g < 48; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(0, 40)).RotatedByRandom(MathHelper.TwoPi);
			var splash = new IstafelsSunfireSplash
			{
				velocity = afterVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(20f), 0).RotatedByRandom(6.283) - afterVelocity,
				maxTime = Main.rand.Next(90, 160),
				scale = Main.rand.NextFloat(6f, 8f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.4f), 0 },
			};
			Ins.VFXManager.Add(splash);
		}
		for (int g = 0; g < 24; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(0, 10)).RotatedByRandom(MathHelper.TwoPi);
			var splash = new IstafelsSunfireSplash
			{
				velocity = afterVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(20f), 0).RotatedByRandom(6.283) - afterVelocity,
				maxTime = Main.rand.Next(72, 144),
				scale = Main.rand.NextFloat(16f, 24f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.4f), 0 },
			};
			Ins.VFXManager.Add(splash);
		}
		Projectile.hide = true;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<Charred>(), IstafelsSunfireGrasp_FireBall.BuffDuration);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float range = 120;
		bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < range;
		bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < range;
		bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < range;
		bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < range;
		return bool0 || bool1 || bool2 || bool3;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
		base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var drawPos = Projectile.Center - Main.screenPosition;
		var duration = 1 - Projectile.timeLeft / 120f;
		var fade = Projectile.timeLeft / 120f;
		var drawScale = MathHelper.Lerp(4f, 36f, duration);
		var drawSize = MathF.Sqrt(Projectile.width / 2 * drawScale) * 0.27f;
		var timeValue = MathF.Pow(Timer, 0.5f) * 0.03f;
		float resolutionCircle = 50;
		var bars = new List<Vertex2D>();
		var bars2 = new List<Vertex2D>();

		var bars_dark = new List<Vertex2D>();
		var bars2_dark = new List<Vertex2D>();
		for (int i = 0; i <= resolutionCircle; i++)
		{
			var radiusInner = new Vector2(0, 4 * drawSize).RotatedBy(i / resolutionCircle * MathHelper.TwoPi);
			var radiusMiddle = new Vector2(0, 24 * drawSize).RotatedBy(i / resolutionCircle * MathHelper.TwoPi) - Projectile.velocity;
			var radiusOuter = new Vector2(0, 90 * drawSize).RotatedBy(i / resolutionCircle * MathHelper.TwoPi) - Projectile.velocity * 2;
			var colorInner = new Color(0, 0, 0, 0);
			Color colorMiddle = new Color(fade, MathF.Pow(fade, 2) * 0.6f, MathF.Pow(fade, 3) * 0.4f, 0) * MathF.Pow(fade, 0.5f);
			Color colorMiddle_dark = Color.White * MathF.Pow(fade, 0.85f);
			var colorOuter = new Color(0, 0, 0, 0);

			bars.Add(drawPos + radiusMiddle, colorMiddle, new Vector3(i / 50f, timeValue, 1));
			bars.Add(drawPos + radiusInner, colorInner, new Vector3(i / 50f, timeValue + 0.15f, 1));

			bars2.Add(drawPos + radiusMiddle, colorMiddle, new Vector3(i / 50f, timeValue, 1));
			bars2.Add(drawPos + radiusOuter, colorOuter, new Vector3(i / 50f, timeValue - 0.15f, 1));

			bars_dark.Add(drawPos + radiusMiddle, colorMiddle_dark, new Vector3(i / 50f, timeValue, 1));
			bars_dark.Add(drawPos + radiusInner, colorInner, new Vector3(i / 50f, timeValue + 0.15f, 1));

			bars2_dark.Add(drawPos + radiusMiddle, colorMiddle_dark, new Vector3(i / 50f, timeValue, 1));
			bars2_dark.Add(drawPos + radiusOuter, colorOuter, new Vector3(i / 50f, timeValue - 0.15f, 1));
		}

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_melting_black.Value;
		if (bars_dark.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_dark.ToArray(), 0, bars_dark.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_dark.ToArray(), 0, bars_dark.Count - 2);
		}

		if (bars2_dark.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2_dark.ToArray(), 0, bars2_dark.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2_dark.ToArray(), 0, bars2_dark.Count - 2);
		}
		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_melting.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		if (bars2.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		var drawPos = Projectile.Center - Main.screenPosition;
		var duration = 1 - Projectile.timeLeft / 120f;
		var fade = Projectile.timeLeft / 120f;
		var drawScale = MathHelper.Lerp(4f, 16f, duration);
		var drawSize = MathF.Sqrt(Projectile.width / 2 * drawScale) * 0.27f;
		var timeValue = (float)Main.timeForVisualEffects * 0.003f;
		float resolutionCircle = 50;
		var bars = new List<Vertex2D>();
		var bars2 = new List<Vertex2D>();
		for (int i = 0; i <= resolutionCircle; i++)
		{
			var radiusInner = new Vector2(0, 4 * drawSize).RotatedBy(i / resolutionCircle * MathHelper.TwoPi);
			var radiusMiddle = new Vector2(0, 24 * drawSize).RotatedBy(i / resolutionCircle * MathHelper.TwoPi) - Projectile.velocity;
			var radiusOuter = new Vector2(0, 90 * drawSize).RotatedBy(i / resolutionCircle * MathHelper.TwoPi) - Projectile.velocity * 2;

			float rot = MathHelper.PiOver2;

			Vector2 dirInner = new Vector2(1, 0).RotatedBy(i / resolutionCircle * MathHelper.TwoPi + rot);
			Vector2 dirMiddle = (dirInner - Projectile.velocity.NormalizeSafe() * 0.15f).NormalizeSafe();
			Vector2 dirOuter = (dirMiddle - Projectile.velocity.NormalizeSafe() * 0.15f).NormalizeSafe();

			var colorInner = new Color(-dirInner.X * 0.5f + 0.5f, -dirInner.Y * 0.5f + 0.5f, 0, 0);
			var colorMiddle = new Color(-dirMiddle.X * 0.5f + 0.5f, -dirMiddle.Y * 0.5f + 0.5f, fade * 0.12f * Main.GameViewMatrix.Zoom.X, 0);
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
