namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.SquamousShell;

public class Squamous_HitTile : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 120;
		Projectile.height = 120;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 12;
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
	}

	private static void DrawTexCircle(float radious, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radious / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radious % 1, 0.8f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 24 / radious % 1, 0.5f, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), color, new Vector3(1, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(1, 0.5f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), color, new Vector3(0, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(0, 0.5f, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		Texture2D shadow = Commons.ModAsset.Point.Value;
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		var light = lightColor.ToVector4();
		Main.spriteBatch.Draw(shadow, Projectile.Center - Main.screenPosition, null, new Color(1f * (1 - timeValue) * (1 - timeValue), 1f * (1 - timeValue), 1f, 0f) * dark, 0, shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f * dark, SpriteEffects.None, 0);
		DrawTexCircle(MathF.Sqrt(timeValue) * 24 * Projectile.ai[0], 8 * (1 - timeValue) * Projectile.ai[0], lightColor * (1 - timeValue) * 0.75f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_black.Value);
		DrawTexCircle(MathF.Sqrt(timeValue) * 24 * Projectile.ai[0], 8 * (1 - timeValue) * Projectile.ai[0], new Color(0.32f * light.X, 0.18f * light.Y, 0.24f * light.Z, 0f) * (1 - timeValue), Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_6.Value);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D shadow = Commons.ModAsset.Point_black.Value;
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		float dark = Math.Max((Projectile.timeLeft - 50) / 150f, 0);
		Main.spriteBatch.Draw(shadow, Projectile.Center - Main.screenPosition, null, Color.White * dark, 0, shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f, SpriteEffects.None, 0);
		Texture2D light = Commons.ModAsset.Textures_Star.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(1f * (1 - timeValue), 0.4f * (1 - timeValue), 0.4f * (1 - timeValue), 0f), 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * Projectile.ai[0] / 4f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(1f * (1 - timeValue), 0.4f * (1 - timeValue), 0.4f * (1 - timeValue), 0f), MathF.PI / 3f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] / 4f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(1f * (1 - timeValue), 0.4f * (1 - timeValue), 0.4f * (1 - timeValue), 0f), MathF.PI / 3f * 2f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] / 4f, SpriteEffects.None, 0);
		return false;
	}

	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radious, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		Color c0 = color;
		c0.R = 0;
		for (int h = 0; h < radious / 2; h += 1)
		{
			c0.R = (byte)(h / radious * 2 * 255);
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(h / radious * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radious, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(h / radious * Math.PI * 4 + addRot), c0, new Vector3(h * 2 / radious, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), c0, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), c0, new Vector3(1, 0, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), c0, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), c0, new Vector3(0, 0, 0)));
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
		float width = 60;
		if (Projectile.timeLeft < 60)
		{
			width = Projectile.timeLeft;
		}

		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 24f * Projectile.ai[0], width * 0.5f, new Color(colorV, colorV * 0.06f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}
}
