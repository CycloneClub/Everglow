using Everglow.Commons.DataStructures;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Yggdrasil.CityOfMagicFlute.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class ChainGrenadeProjExplosion : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 160;
		Projectile.height = 160;
		Projectile.friendly = true;
		Projectile.hostile = true;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 3;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
		Projectile.DamageType = DamageClass.Ranged;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;
		Timer = 0;
	}

	public int Timer = 0;

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
		base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public void LargeFlame(int count)
	{
		for (int x = 0; x < count; x++)
		{
			Vector2 newVelocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 8f).RotatedByRandom(MathHelper.TwoPi);
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
			Vector2 newVelocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 4f).RotatedByRandom(MathHelper.TwoPi);
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

	public void FlameII(int count)
	{
		for (int x = 0; x < count; x++)
		{
			var newVelocity = new Vector2(0, -10);
			Vector2 addPos = new Vector2(Main.rand.NextFloat(40), 0).RotatedByRandom(MathHelper.TwoPi);
			var fire = new FireDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center + addPos - newVelocity,
				maxTime = Main.rand.Next(30, 45),
				scale = Main.rand.NextFloat(20f, 60f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), addPos.X * 0.002f },
			};
			Ins.VFXManager.Add(fire);
		}
	}

	public void Spark(int count)
	{
		for (int x = 0; x < count; x++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 22f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new FireSparkDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(6.283) + newVelocity * 3,
				maxTime = Main.rand.Next(10, 20),
				scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(1f, 25.0f)),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	public override void AI()
	{
		Timer++;
		if (Timer > 5)
		{
			Projectile.friendly = false;
		}
		Projectile.hide = true;
		Projectile.velocity *= 0;
		if (Timer == 10)
		{
			Spark(120);
			LargeFlame(35);
			SmallFlame(75);
		}

		if (Timer == 20)
		{
			FlameII(35);
		}
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

	public void DrawWarp(VFXBatch spriteBatch)
	{
		// TODO : Bug
		// var drawPos = Projectile.Center - Main.screenPosition;
		// var duration = Timer / 200f;
		// var fade = Projectile.timeLeft / 200f;
		// var drawScale = MathHelper.Lerp(1f, 9f, duration);
		// var drawSize = MathF.Sqrt(Projectile.width / 2 * drawScale) * 0.27f;
		// var timeValue = (float)Main.timeForVisualEffects * 0.003f;
		// float resolutionCircle = 50;
		// var bars = new List<Vertex2D>();
		// var bars2 = new List<Vertex2D>();
		// for (int i = 0; i <= resolutionCircle; i++)
		// {
		// var radiusInner = new Vector2(0, 4 * drawSize).RotatedBy(i / resolutionCircle * MathHelper.TwoPi);
		// var radiusMiddle = new Vector2(0, 24 * drawSize).RotatedBy(i / resolutionCircle * MathHelper.TwoPi);
		// var radiusOuter = new Vector2(0, 90 * drawSize).RotatedBy(i / resolutionCircle * MathHelper.TwoPi);

		// float rot = MathHelper.PiOver2;

		// Vector2 dirInner = new Vector2(1, 0).RotatedBy(i / resolutionCircle * MathHelper.TwoPi + rot);
		// Vector2 dirMiddle = dirInner.NormalizeSafe();
		// Vector2 dirOuter = dirMiddle.NormalizeSafe();

		// var colorInner = new Color(-dirInner.X * 0.5f + 0.5f, -dirInner.Y * 0.5f + 0.5f, 0, 0);
		// var colorMiddle = new Color(-dirMiddle.X * 0.5f + 0.5f, -dirMiddle.Y * 0.5f + 0.5f, fade * 1.2f, 0);
		// var colorOuter = new Color(-dirOuter.X * 0.5f + 0.5f, -dirOuter.Y * 0.5f + 0.5f, 0, 0);
		// bars.Add(drawPos + radiusMiddle, colorMiddle, new Vector3(timeValue, i / 50f, 1));
		// bars.Add(drawPos + radiusInner, colorInner, new Vector3(timeValue + 0.15f, i / 50f, 1));

		// bars2.Add(drawPos + radiusMiddle, colorMiddle, new Vector3(timeValue, i / 50f, 1));
		// bars2.Add(drawPos + radiusOuter, colorOuter, new Vector3(timeValue - 0.15f, i / 50f, 1));
		// }
		// Main.graphics.graphicsDevice.BlendState = BlendState.NonPremultiplied;
		// Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		// Main.graphics.graphicsDevice.RasterizerState = RasterizerState.CullNone;
		// if (bars.Count > 2)
		// {
		// spriteBatch.Draw(Commons.ModAsset.Noise_melting_H.Value, bars, PrimitiveType.TriangleStrip);
		// }
		// if (bars2.Count > 2)
		// {
		// spriteBatch.Draw(Commons.ModAsset.Noise_melting_H.Value, bars2, PrimitiveType.TriangleStrip);
		// }
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void OnKill(int timeLeft) => base.OnKill(timeLeft);

	public override bool PreDraw(ref Color lightColor)
	{
		var drawPos = Projectile.Center - Main.screenPosition;
		var duration = Timer / 200f;
		var fade = Projectile.timeLeft / 200f;
		var drawScale = MathHelper.Lerp(1f, 9f, duration);
		var drawSize = MathF.Sqrt(Projectile.width / 2 * drawScale) * 0.12f;
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
}
