using Everglow.Commons.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

public class LanternFlowLine : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
		Projectile.width = 120;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 600;
		Projectile.penetrate = -1;
		Projectile.scale = 0;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 14400;
	}

	public override void AI()
	{
		if (Projectile.timeLeft > 500)
		{
			Projectile.scale += 0.01f;
		}
		if (Projectile.timeLeft < 50)
		{
			Projectile.scale -= 0.02f;
		}
		Projectile.scale = Math.Clamp(Projectile.scale, 0, 1);
		if (Projectile.timeLeft > 120 && Projectile.timeLeft < 600)
		{
			float mulScale = Main.rand.NextFloat(0.5f, 1.2f);
			var gore2 = new LanternFlow_lantern2
			{
				Active = true,
				Visible = true,
				velocity = new Vector2(Main.rand.NextFloat(-1.2f, 1.2f), -5 * mulScale),
				scale = mulScale,
				position = Projectile.Center + new Vector2(0, Main.rand.NextFloat(-1400f, -600f)),
			};
			Ins.VFXManager.Add(gore2);
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return targetHitbox.Left < projHitbox.Right + 60 * Projectile.scale && targetHitbox.Right > projHitbox.Left - 60 * Projectile.scale && Math.Abs(targetHitbox.Center.Y - projHitbox.Center.Y) < 2000 && Projectile.timeLeft < 550 && Projectile.timeLeft > 50;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		float timeValue = (float)(-Main.time * 0.02f) + Projectile.whoAmI / 7f;
		Vector2 drawPoint = new Vector2(0, -2000) + Projectile.Center;
		float mulColor = 1;
		if (Projectile.timeLeft > 500)
		{
			mulColor *= (600 - Projectile.timeLeft) / 100f;
		}

		Color darkColor = new Color(0f, 0f, 0f, 1f) * mulColor;
		var bars_Dark = new List<Vertex2D>();
		var bars = new List<Vertex2D>();

		var bars_left_Dark = new List<Vertex2D>();
		var bars_left = new List<Vertex2D>();
		for (int y = 0; y < 600; y++)
		{
			float value0 = MathF.Sin((drawPoint.X - 150 * Projectile.scale + drawPoint.Y - (float)Main.time * 30) * 0.01f) * 0.5f + 0.5f;
			float value1 = MathF.Sin((drawPoint.X + drawPoint.Y - (float)Main.time * 30) * 0.01f) * 0.5f + 0.5f;
			float value2 = MathF.Sin((drawPoint.X + 150 * Projectile.scale + drawPoint.Y - (float)Main.time * 30) * 0.01f) * 0.5f + 0.5f;
			Color c0 = Color.Lerp(new Color(0.5f, 0f, 0.1f, 0f), new Color(1f, 0.3f, 0.04f, 0f), value0);
			Color c1 = Color.Lerp(new Color(0.5f, 0f, 0.1f, 0f), new Color(1f, 0.3f, 0.04f, 0f), value1);
			Color c2 = Color.Lerp(new Color(0.5f, 0f, 0.1f, 0f), new Color(1f, 0.3f, 0.04f, 0f), value2);

			float coordY = MathF.Pow(y * 4, 0.5f) + timeValue;
			var drawPos = drawPoint - Main.screenPosition;
			bars.Add(drawPos + new Vector2(0, 0), c1, new Vector3(coordY, 1 - 0.5f * Projectile.scale, 0));
			bars.Add(drawPos + new Vector2(150 * Projectile.scale, 0), c2, new Vector3(coordY, 1, 0));

			bars_Dark.Add(drawPos + new Vector2(0, 0), darkColor, new Vector3(coordY, 1 - 0.5f * Projectile.scale, 0));
			bars_Dark.Add(drawPos + new Vector2(150 * Projectile.scale, 0), darkColor, new Vector3(coordY, 1, 0));

			bars_left.Add(drawPos + new Vector2(0, 0), c1, new Vector3(coordY, 0.5f * Projectile.scale, 0));
			bars_left.Add(drawPos + new Vector2(-150 * Projectile.scale, 0), c0, new Vector3(coordY, 0, 0));

			bars_left_Dark.Add(drawPos + new Vector2(0, 0), darkColor, new Vector3(coordY, 0.5f * Projectile.scale, 0));
			bars_left_Dark.Add(drawPos + new Vector2(-150 * Projectile.scale, 0), darkColor, new Vector3(coordY, 0, 0));

			drawPoint += new Vector2(0, 30);
			if (y > 20)
			{
				drawPoint += new Vector2(0, MathF.Min(30 + (y - 20), 150));
			}
			if (!VFXManager.InScreen(drawPoint, 500) && VFXManager.InScreen(drawPoint + new Vector2(0, -100), 500))
			{
				break;
			}
		}
		if (bars_left_Dark.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_19_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_left_Dark.ToArray(), 0, bars_left_Dark.Count - 2);
		}
		if (bars_Dark.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_19_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_Dark.ToArray(), 0, bars_Dark.Count - 2);
		}
		int lightValue = 1;
		if (Projectile.timeLeft > 550 && Projectile.timeLeft < 560)
		{
			lightValue = Projectile.timeLeft - 550;
		}
		for (int k = 0; k < lightValue; k++)
		{
			if (bars_left.Count > 2)
			{
				Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_19.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_left.ToArray(), 0, bars_left.Count - 2);
			}
			if (bars.Count > 2)
			{
				Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_19.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			}
		}

		drawPoint = new Vector2(0, -2000) + Projectile.Center;
		bars = new List<Vertex2D>();
		for (int y = 0; y < 600; y++)
		{
			bars.Add(drawPoint - Main.screenPosition + new Vector2(150 * Projectile.scale, 0), new Color(0.3f, 0f, 0.05f, 0) * mulColor, new Vector3(y / 40f + timeValue, 0, 0));
			bars.Add(drawPoint - Main.screenPosition + new Vector2(-150 * Projectile.scale, 0), new Color(0.3f, 0f, 0.05f, 0) * mulColor, new Vector3(y / 40f + timeValue, 1, 0));
			drawPoint += new Vector2(0, 30);
			if (y > 20)
			{
				drawPoint += new Vector2(0, MathF.Min(30 + (y - 20), 150));
			}
			if (!VFXManager.InScreen(drawPoint, 500) && VFXManager.InScreen(drawPoint + new Vector2(0, -100), 500))
			{
				break;
			}
		}
		if (bars.Count > 2)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_thick.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
	}
}
