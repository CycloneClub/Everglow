using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class BlackCorruptRain3Friendly : ModProjectile
{
	public override string Texture => "Everglow/Myth/TheFirefly/Projectiles/BlackCorruptRain3";

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Black Corrupt Ball");
	}

	public override void SetDefaults()
	{
		Projectile.width = 8;
		Projectile.height = 8;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 2;
		Projectile.timeLeft = 700;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 1f;
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color?(new Color(255, 255, 255, 0));
	}

	public override void AI()
	{
		if (Projectile.velocity.Length() < 5f)
		{
			Projectile.velocity *= 1.018f;
		}

		Lighting.AddLight(Projectile.Center, 0, 0.4f, 0.9f);
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D t = Commons.ModAsset.Trail_2.Value;
		float width = 20;
		if (Projectile.timeLeft < 120)
		{
			width = Projectile.timeLeft / 6f;
		}

		Ins.Batch.Begin();
		DrawTexCircle_VFXBatch(Ins.Batch, 30 + 7 * MathF.Sin((float)(Main.timeForVisualEffects / 3f + Projectile.ai[0])), width, new Color(0, 150, 255, 0) * 0.4f, Projectile.Center - Main.screenPosition, t, (float)(Main.timeForVisualEffects / 3.8f + Projectile.ai[0]));
		Ins.Batch.End();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D Light = ModAsset.FixCoinLight3.Value;
		Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, Light.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		return true;
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
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(0, 0.2f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 0.8f, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}
}
