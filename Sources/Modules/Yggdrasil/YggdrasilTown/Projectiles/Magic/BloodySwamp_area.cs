using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class BloodySwamp_area : ModProjectile, IWarpProjectile_warpStyle2
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override string Texture => Commons.ModAsset.Empty_Mod;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 1200;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.ignoreWater = true;
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override void OnSpawn(IEntitySource source)
	{
		for (int i = 0; i < 60; i++)
		{
			float rotSpeed = 0;
			Vector2 vel = new Vector2(4 + MathF.Sin(i / (60f / 24f) * MathHelper.TwoPi), 0).RotatedBy(i / 60f * MathHelper.TwoPi);
			var dustVFX = new Heart_VFX
			{
				Omega = rotSpeed,
				beta = -rotSpeed * 0.05f,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				Velocity = vel,
				MaxTime = vel.Length() * 12,
				Scale = 9f,
				DustColor = Color.Red,
				ai = new float[] { Main.rand.NextFloat(1f, 8f) },
			};
			Ins.VFXManager.Add(dustVFX);
		}
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		var colorLight = new Color(0.9f, 0.0f, Main.rand.NextFloat(0.12f), 1);
		float scaleMul = Main.rand.NextFloat(75, 115);
		if (Projectile.timeLeft > 60)
		{
			var dustVFX = new Heart_VFX_spin
			{
				Omega = 0.01f + scaleMul * 0.0002f,
				rotatedCenter = Projectile.Center,
				Radius = scaleMul,
				RotPos = Main.rand.NextFloat(MathHelper.TwoPi),
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(70, 120),
				MaxScale = scaleMul / 12f + Main.rand.NextFloat(-2, 2),
				Scale = Main.rand.Next(4, 5),
				DustColor = colorLight,
				ai = new float[] { Main.rand.NextFloat(1f, 8f) },
			};
			Ins.VFXManager.Add(dustVFX);
		}
		if (Projectile.timeLeft % 60 == 0)
		{
			foreach (Player player in Main.player)
			{
				if ((player.Center - Projectile.Center).Length() < 110)
				{
					player.Heal(5);
				}
			}
		}
		CheckExtra();
	}

	public void CheckExtra()
	{
		Player player = Main.player[Projectile.owner];
		if (player != null)
		{
			if (player.ownedProjectileCounts[Type] > 1)
			{
				foreach (Projectile projectile in Main.projectile)
				{
					if (projectile != null && projectile != Projectile)
					{
						if (projectile.active && projectile.type == Type && projectile.owner == Projectile.owner)
						{
							if (projectile.timeLeft < Projectile.timeLeft && projectile.timeLeft > 60)
							{
								projectile.timeLeft = 60;
							}
						}
					}
				}
			}
		}
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var drawPos = Projectile.Center - Main.screenPosition;
		var fade = 1f;
		if (Projectile.timeLeft < 60f)
		{
			fade = Projectile.timeLeft / 60f;
		}
		if (Projectile.timeLeft > 1140)
		{
			fade = (1200 - Projectile.timeLeft) / 60f;
			fade = MathF.Pow(fade, 3f);
		}
		var size = 1f;
		if (Projectile.timeLeft > 1182)
		{
			size = (1200 - Projectile.timeLeft) / 15f;
			size = MathF.Pow(size, 0.3f);
		}
		var drawColor = new Color(1f, 0.1f, 0.3f, 0) * fade;
		var blackColor = Color.White * fade;
		var timeValue = (float)Main.time * 0.003f;

		var bars_black = new List<Vertex2D>();
		for (int i = 0; i <= 150; i++)
		{
			var radius = new Vector2(0, 90 * size).RotatedBy(i / 150f * MathHelper.TwoPi);
			bars_black.Add(drawPos + radius, blackColor, new Vector3(i / 50f, timeValue, 0));
			bars_black.Add(drawPos, blackColor * 0, new Vector3(i / 50f + 0.6f, timeValue + 0.3f, 0));
		}
		var bars_outside_black = new List<Vertex2D>();
		for (int i = 0; i <= 150; i++)
		{
			var radius = new Vector2(0, 90 * size).RotatedBy(i / 150f * MathHelper.TwoPi);
			var radius2 = new Vector2(0, 120 * size).RotatedBy(i / 150f * MathHelper.TwoPi);
			bars_outside_black.Add(drawPos + radius2, blackColor * 0, new Vector3(i / 50f - 0.6f, timeValue - 0.3f, 0));
			bars_outside_black.Add(drawPos + radius, blackColor, new Vector3(i / 50f, timeValue, 0));
		}

		var bars = new List<Vertex2D>();
		for (int i = 0; i <= 150; i++)
		{
			var radius = new Vector2(0, 90 * size).RotatedBy(i / 150f * MathHelper.TwoPi);
			bars.Add(drawPos + radius, new Color(0.2f, 0f, 0.2f, 0f), new Vector3(i / 50f, timeValue, 0));
			bars.Add(drawPos, drawColor * 0, new Vector3(i / 50f + 0.6f, timeValue + 0.3f, 0));
		}
		var bars_outside = new List<Vertex2D>();
		for (int i = 0; i <= 150; i++)
		{
			var radius = new Vector2(0, 90 * size).RotatedBy(i / 150f * MathHelper.TwoPi);
			var radius2 = new Vector2(0, 120 * size).RotatedBy(i / 150f * MathHelper.TwoPi);
			bars_outside.Add(drawPos + radius2, drawColor * 0, new Vector3(i / 50f - 0.6f, timeValue - 0.3f, 0));
			bars_outside.Add(drawPos + radius, drawColor, new Vector3(i / 50f, timeValue, 0));
		}

		var bars_side = new List<Vertex2D>();
		for (int i = 0; i <= 150; i++)
		{
			var radius = new Vector2(0, 90 * size).RotatedBy(i / 150f * MathHelper.TwoPi);
			var radius2 = new Vector2(0, 90 * size + 10 * fade).RotatedBy(i / 150f * MathHelper.TwoPi);
			bars_side.Add(drawPos + radius, new Color(1f, 0f, 0.2f, 0f), new Vector3(i / 10f, 0, 0));
			bars_side.Add(drawPos + radius2, new Color(1f, 0f, 0.2f, 0f), new Vector3(i / 10f, 1, 0));
			if (i % 5 == 0)
			{
				Lighting.AddLight(Projectile.Center + radius2, fade * 0.45f, 0, 0);
			}
		}

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_smoke_black.Value;
		if (bars_black.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_black.ToArray(), 0, bars_black.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_black.ToArray(), 0, bars_black.Count - 2);
		}
		if (bars_outside_black.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_outside_black.ToArray(), 0, bars_outside_black.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_outside_black.ToArray(), 0, bars_outside_black.Count - 2);
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_smoke.Value;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		if (bars_outside.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_outside.ToArray(), 0, bars_outside.Count - 2);
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Wave_full.Value;
		if (bars_side.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_side.ToArray(), 0, bars_side.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		var drawPos = Projectile.Center - Main.screenPosition;
		var fade = 1f;
		if (Projectile.timeLeft < 60f)
		{
			fade *= Projectile.timeLeft / 60f;
		}
		if (Projectile.timeLeft > 1140)
		{
			fade = (1200 - Projectile.timeLeft) / 60f;
			fade = MathF.Pow(fade, 3f);
		}
		var size = 1f;
		if (Projectile.timeLeft > 1182)
		{
			size = (1200 - Projectile.timeLeft) / 15f;
			size = MathF.Pow(size, 0.3f);
		}
		var timeValue = (float)Main.time * 0.003f;
		var bars = new List<Vertex2D>();
		var bars2 = new List<Vertex2D>();
		for (int i = 0; i <= 150; i++)
		{
			var radiusInner = new Vector2(0, 20 * size).RotatedBy(i / 150f * MathHelper.TwoPi);
			var radiusMiddle = new Vector2(0, 90 * size).RotatedBy(i / 150f * MathHelper.TwoPi);
			var radiusOuter = new Vector2(0, 110 * size).RotatedBy(i / 150f * MathHelper.TwoPi);

			float rot = MathHelper.PiOver2;

			Vector2 dirInner = new Vector2(1, 0).RotatedBy(i / 150f * MathHelper.TwoPi + rot);
			Vector2 dirMiddle = dirInner.RotatedBy(0.6f);
			Vector2 dirOuter = dirMiddle.RotatedBy(0.3f);

			var colorInner = new Color(-dirInner.X * 0.5f + 0.5f, -dirInner.Y * 0.5f + 0.5f, 0, 0);
			var colorMiddle = new Color(-dirMiddle.X * 0.5f + 0.5f, -dirMiddle.Y * 0.5f + 0.5f, fade * 0.1f, 0);
			var colorOuter = new Color(-dirOuter.X * 0.5f + 0.5f, -dirOuter.Y * 0.5f + 0.5f, 0, 0);

			bars.Add(drawPos + radiusMiddle, colorMiddle, new Vector3(i / 50f - timeValue, timeValue, 0));
			bars.Add(drawPos + radiusInner, colorInner, new Vector3(i / 50f + 0.6f - timeValue, timeValue + 0.3f, 0));

			bars2.Add(drawPos + radiusOuter, colorOuter, new Vector3(i / 50f - 0.6f - timeValue, timeValue - 0.3f, 0));
			bars2.Add(drawPos + radiusMiddle, colorMiddle, new Vector3(i / 50f - timeValue, timeValue, 0));
		}
		if (bars.Count > 2)
		{
			Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.graphicsDevice.RasterizerState = RasterizerState.CullNone;
			spriteBatch.Draw(Commons.ModAsset.Noise_smoke.Value, bars, PrimitiveType.TriangleStrip);
		}
		if (bars2.Count > 2)
		{
			Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.graphicsDevice.RasterizerState = RasterizerState.CullNone;
			spriteBatch.Draw(Commons.ModAsset.Noise_smoke.Value, bars2, PrimitiveType.TriangleStrip);
		}
	}
}
