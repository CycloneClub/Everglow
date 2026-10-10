using Everglow.Myth.Common;

namespace Everglow.Myth.OmniElementItems.Projectiles;

public class LilyHarpNoteKill : ModProjectile// , IWarpProjectile
{
	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.BehindProjectiles;
		Projectile.extraUpdates = 1;
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
		float value = (200 - Projectile.timeLeft) / (float)Projectile.timeLeft * 1.4f;

		if (value < 1)
		{
			DrawCircle(value * 110, 15 * (1 - value) + 3, new Color(0, 0.15f * (1 - value), 0.03f * (1 - value), 0f), Projectile.Center - Main.screenPosition);
		}

		value -= 0.2f;
		if (value is < 1 and > 0)
		{
			DrawCircle(value * 90, 8 * (1 - value) + 3, new Color(0, 0.10f * (1 - value), 0.06f * (1 - value), 0f), Projectile.Center - Main.screenPosition);
		}

		return false;
	}


	private static void DrawCircle(float radius, float width, Color color, Vector2 center)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4), color, new Vector3(0.5f, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 4), color, new Vector3(0.5f, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, radius), color, new Vector3(0.5f, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
		{
			Texture2D t = ModAsset.Wave.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	private static void DrawCircle(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4), color, new Vector3(0.5f, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 4), color, new Vector3(0.5f, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, radius), color, new Vector3(0.5f, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
		{
			Texture2D t = ModAsset.Wave.Value;
			spriteBatch.Draw(t, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = (200 - Projectile.timeLeft) / (float)Projectile.timeLeft * 1.4f;

		if (value < 1)
		{
			DrawCircle(spriteBatch, value * 110, 15 * (1 - value) + 3, new Color(0, 0.15f * (1 - value), 0.03f * (1 - value), 0f), Projectile.Center - Main.screenPosition);
		}

		value -= 0.2f;
		if (value is < 1 and > 0)
		{
			DrawCircle(spriteBatch, value * 90, 8 * (1 - value) + 3, new Color(0, 0.10f * (1 - value), 0.06f * (1 - value), 0f), Projectile.Center - Main.screenPosition);
		}
	}
}
