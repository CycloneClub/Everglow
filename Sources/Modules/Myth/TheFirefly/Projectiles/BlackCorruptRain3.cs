namespace Everglow.Myth.TheFirefly.Projectiles;

public class BlackCorruptRain3 : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 8;
		Projectile.height = 8;
		Projectile.aiStyle = -1;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 2;
		Projectile.timeLeft = 700;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 0;
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color?(new Color(255, 255, 255, 0));
	}

	public override void AI()
	{
		if (Projectile.scale < 1)
		{
			Projectile.scale += 0.02f;
		}
		if (Projectile.velocity.Length() < 5f)
		{
			Projectile.velocity *= 1.018f;
		}

		Lighting.AddLight(Projectile.Center, 0, 0.4f, 0.9f);
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		float width = 20;
		if (Projectile.timeLeft < 120)
		{
			width = Projectile.timeLeft / 6f;
		}

		Ins.Batch.Begin();
		DrawTexCircle_VFXBatch(Ins.Batch, (30 + 7 * MathF.Sin((float)(Main.time / 3f + Projectile.ai[0]))) * Projectile.scale, width * Projectile.scale, Color.White * 0.1f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_2_black_thick.Value, (float)(Main.time / 3.8 + Projectile.ai[0]));
		DrawTexCircle_VFXBatch(Ins.Batch, (30 + 7 * MathF.Sin((float)(Main.time / 3f + Projectile.ai[0]))) * Projectile.scale, width * Projectile.scale, new Color(0, 150, 255, 0) * 0.4f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_2.Value, (float)(Main.time / 3.8 + Projectile.ai[0]));
		Ins.Batch.End();
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D light = ModAsset.FixCoinLight3.Value;
		Texture2D dark = Commons.ModAsset.Point_black.Value;
		Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f) * 0.7f, Projectile.rotation, dark.Size() / 2f, Projectile.scale * 0.25f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, light.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		return false;
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
