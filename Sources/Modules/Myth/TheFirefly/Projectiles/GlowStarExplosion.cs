namespace Everglow.Myth.TheFirefly.Projectiles;

public class GlowStarExplosion : ModProjectile, IWarpProjectile
{
	public override string Texture => ModAsset.GlowStar_Mod;

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindProjectiles;
		Projectile.extraUpdates = 3;
		Projectile.width = 24;
		Projectile.height = 24;
		Projectile.hostile = false;
		Projectile.friendly = false;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 200;
		Projectile.aiStyle = -1;
	}

	public override void AI()
	{
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		value = MathF.Sqrt(value);
		float colorV = 0.9f * (1 - value);
		colorV *= 10;
		Texture2D t = Commons.ModAsset.Trail_1.Value;
		Texture2D shade = Commons.ModAsset.Trail_2_black_thick.Value;
		float width = 120;
		if (Projectile.timeLeft < 120)
		{
			width = Projectile.timeLeft;
		}

		Ins.Batch.Begin();
		DrawTexCircle_VFXBatch(Ins.Batch, value * 270 * Projectile.ai[0], width, new Color(0, colorV * 0.2f, colorV, 0f) * 0.4f, Projectile.Center - Main.screenPosition, t, Projectile.ai[0] * 25f);
		DrawTexCircle_VFXBatch(Ins.Batch, value * 200 * Projectile.ai[0], width, Color.White, Projectile.Center - Main.screenPosition, shade, Main.timeForVisualEffects * 0.03f + Projectile.ai[0] * 25f);

		Ins.Batch.End();
		return false;
	}


	private static void DrawWarpTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		for (int h = 0; h < radius / 2; h += 1)
		{
			float colorR = (h / radius * MathF.PI * 4 + (float)addRot + 1.57f) % (MathF.PI * 2f) / (MathF.PI * 2f);
			float color2R = ((h + 1) / radius * MathF.PI * 4 + (float)addRot + 1.57f) % (MathF.PI * 2f) / (MathF.PI * 2f);

			color = new Color(colorR, color.G / 255f, 0, 0);
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0.8f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0.2f, 0)));
			if (Math.Abs(color2R - colorR) > 0.8f)
			{
				float midValue = (1f - colorR) / (float)(color2R + (1f - colorR));
				color.R = 255;
				circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy((h + midValue) / radius * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radius, 0.8f, 0)));
				circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy((h + midValue) / radius * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radius, 0.2f, 0)));
				color.R = 0;
				circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy((h + midValue) / radius * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radius, 0.8f, 0)));
				circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy((h + midValue) / radius * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radius, 0.2f, 0)));
			}
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(1, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(1, 0.2f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(0, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 0.2f, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		for (int h = 0; h < radius / 2; h += 1)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0.8f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0.2f, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(1, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(1, 0.2f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(0, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 0.2f, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		value = MathF.Sqrt(value);
		float colorV = 0.9f * (1 - value);
		colorV *= 10;
		Texture2D t = Commons.ModAsset.Trail_2.Value;
		float width = 120;
		if (Projectile.timeLeft < 120)
		{
			width = Projectile.timeLeft;
		}

		DrawWarpTexCircle_VFXBatch(spriteBatch, value * 270 * Projectile.ai[0], width, new Color(colorV, colorV * 0.02f * Projectile.ai[1], colorV, 0f), Projectile.Center - Main.screenPosition, t);
	}
}
