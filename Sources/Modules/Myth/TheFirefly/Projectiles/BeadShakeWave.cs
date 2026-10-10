namespace Everglow.Myth.TheFirefly.Projectiles;

public class BeadShakeWave : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MiscsProjectiles;

	public override void SetDefaults()
	{
		Projectile.extraUpdates = 6;
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
		Projectile.hide = true;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		Color c = new Color(0.2f * MathF.Sqrt(1 - timeValue), 0.6f * (1 - timeValue) * (1 - timeValue), 3f * (1 - timeValue), 0f);
		Color cDark = new Color(0, 0, 0, 1f - timeValue);

		// DrawTexCircle(MathF.Sqrt(timeValue) * 11 * Projectile.ai[0], 20 * (1 - timeValue) * Projectile.ai[0], cDark, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_2_black_thick.Value);
		DrawTexCircle(MathF.Sqrt(timeValue) * 12 * Projectile.ai[0], 4 * (1 - timeValue) * Projectile.ai[0], cDark * 0.5f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_black.Value);
		DrawTexCircle(MathF.Sqrt(timeValue) * 6 * Projectile.ai[0], 3 * (1 - timeValue) * Projectile.ai[0], cDark * 0.5f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_black.Value);

		DrawTexCircle(MathF.Sqrt(timeValue) * 12 * Projectile.ai[0], 4 * (1 - timeValue) * Projectile.ai[0], c * 0.8f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail.Value);
		DrawTexCircle(MathF.Sqrt(timeValue) * 6 * Projectile.ai[0], 3 * (1 - timeValue) * Projectile.ai[0], c * 0.8f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail.Value);

		DrawTexCircle(MathF.Sqrt(timeValue) * 12 * Projectile.ai[0], 1f * (1 - timeValue) * Projectile.ai[0], c * 3f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail.Value);
		DrawTexCircle(MathF.Sqrt(timeValue) * 6 * Projectile.ai[0], 1f * (1 - timeValue) * Projectile.ai[0], c * 3f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail.Value);
		return false;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindProjectiles.Add(index);
	}

	private static void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radius % 1, 0.8f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radius % 1, 0.5f, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(1, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(1, 0.5f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(0, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 0.5f, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		Color c0 = color;
		c0.R = 0;
		for (int h = 0; h < radius / 2; h += 1)
		{
			c0.R = (byte)(h / radius * 2 * 255);
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radius, 0.5f, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), c0, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), c0, new Vector3(1, 0.5f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), c0, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), c0, new Vector3(0, 0.5f, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		float colorV = 0.9f * (1 - value);
		if (Projectile.ai[0] >= 10)
		{
			colorV *= Projectile.ai[0] / 10f;
		}

		Texture2D t = Commons.ModAsset.Trail.Value;

		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 12f * Projectile.ai[0], 6 * (1 - value) * Projectile.ai[0], new Color(colorV, colorV * 0.06f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 6f * Projectile.ai[0], 4 * (1 - value) * Projectile.ai[0], new Color(colorV, colorV * 0.06f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}
}
