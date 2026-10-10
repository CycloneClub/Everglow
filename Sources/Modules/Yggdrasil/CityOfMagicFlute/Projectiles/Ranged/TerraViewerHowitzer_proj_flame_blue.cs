namespace Everglow.Yggdrasil.CityOfMagicFlute.Projectiles.Ranged;

public class TerraViewerHowitzer_proj_flame_blue : ModProjectile, IWarpProjectile, IBloomProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override string Texture => ModAsset.TerraViewerHowitzer_proj_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 32;
		Projectile.height = 32;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 24;
	}

	private static void DrawWarpTexCircle_VFXBatch(VFXBatch spriteBatch, float radious, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		for (int h = 0; h < radious / 2; h += 1)
		{
			float colorR = (h / radious * MathF.PI * 4 + (float)addRot + 1.57f) % (MathF.PI * 2f) / (MathF.PI * 2f);
			float color2R = ((h + 1) / radious * MathF.PI * 4 + (float)addRot + 1.57f) % (MathF.PI * 2f) / (MathF.PI * 2f);

			color = new Color(colorR, color.G / 255f, 0, 0);
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radious, 0.8f, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radious, 0.2f, 0)));
			if (Math.Abs(color2R - colorR) > 0.8f)
			{
				float midValue = (1f - colorR) / (float)(color2R + (1f - colorR));
				color.R = 255;
				circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy((h + midValue) / radious * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radious, 0.8f, 0)));
				circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy((h + midValue) / radious * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radious, 0.2f, 0)));
				color.R = 0;
				circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy((h + midValue) / radious * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radious, 0.8f, 0)));
				circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy((h + midValue) / radious * Math.PI * 4 + addRot), color, new Vector3((h + midValue) * 2 / radious, 0.2f, 0)));
			}
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), color, new Vector3(1, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(1, 0.2f, 0)));
		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	private static void DrawTexFlame_VFXBatch(VFXBatch spriteBatch, float length, float width, Color color, Vector2 center, Texture2D tex, double rotation, float process)
	{
		var flame = new List<Vertex2D>();

		flame.Add(new Vertex2D(center + new Vector2(0, width / 2f).RotatedBy(rotation), Color.Transparent, new Vector3(process % 1f, 0.8f, 0)));
		flame.Add(new Vertex2D(center - new Vector2(0, width / 2f).RotatedBy(rotation), Color.Transparent, new Vector3(process % 1f, 0.2f, 0)));
		for (int h = 1; h < length; h += 6)
		{
			if (Collision.SolidCollision(center + Main.screenPosition + new Vector2(h, 0).RotatedBy(rotation), 0, 0))
			{
				break;
			}
			float widthII = width;
			if (length - h < 120)
			{
				widthII = width * (length - h) / 120f;
			}

			float processII = (process + h / 150f) % 1f;

			flame.Add(new Vertex2D(center + new Vector2(h, 0).RotatedBy(rotation) + new Vector2(0, widthII / 2f).RotatedBy(rotation), color, new Vector3(processII, 0.8f, 0)));
			flame.Add(new Vertex2D(center + new Vector2(h, 0).RotatedBy(rotation) - new Vector2(0, widthII / 2f).RotatedBy(rotation), color, new Vector3(processII, 0.2f, 0)));
			float processIII = (process + (h + 6) / 150f) % 1f;
			if (processIII < processII)
			{
				float midValue = (1 - processII) / (processIII + 1 - processII);
				flame.Add(new Vertex2D(center + new Vector2(h + midValue, 0).RotatedBy(rotation) + new Vector2(0, widthII / 2f).RotatedBy(rotation), color, new Vector3(1, 0.8f, 0)));
				flame.Add(new Vertex2D(center + new Vector2(h + midValue, 0).RotatedBy(rotation) - new Vector2(0, widthII / 2f).RotatedBy(rotation), color, new Vector3(1, 0.2f, 0)));
				flame.Add(new Vertex2D(center + new Vector2(h + midValue, 0).RotatedBy(rotation) + new Vector2(0, widthII / 2f).RotatedBy(rotation), color, new Vector3(0, 0.8f, 0)));
				flame.Add(new Vertex2D(center + new Vector2(h + midValue, 0).RotatedBy(rotation) - new Vector2(0, widthII / 2f).RotatedBy(rotation), color, new Vector3(0, 0.2f, 0)));
			}
		}

		if (flame.Count > 2)
		{
			spriteBatch.Draw(tex, flame, PrimitiveType.TriangleStrip);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		value = MathF.Sqrt(value);
		float colorV = 0.9f * (1 - value);
		Texture2D t = Commons.ModAsset.Trail_2_thick.Value;
		float width = 120;
		if (Projectile.timeLeft < 120)
		{
			width = Projectile.timeLeft;
		}

		Ins.Batch.Begin();
		DrawTexFlame_VFXBatch(Ins.Batch, value * 3540 * Projectile.ai[0], width * 0.2f, new Color(colorV * colorV * 0.1f, colorV * colorV * 1.6f, colorV * 6f, 0f), Projectile.Center - Main.screenPosition, t, Projectile.ai[1], (float)-Main.timeForVisualEffects * 0.32f + 1000000 + Projectile.ai[1] * 200);
		Ins.Batch.End();
		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		value = MathF.Sqrt(value);
		float colorV = 0.9f * (1 - value);
		Texture2D t = Commons.ModAsset.Trail_2_thick.Value;
		float width = 120;
		if (Projectile.timeLeft < 120)
		{
			width = Projectile.timeLeft;
		}

		DrawWarpTexCircle_VFXBatch(spriteBatch, value * 160 * Projectile.ai[0], width * 0.2f, new Color(colorV, colorV * 0.07f, colorV, 0f), Projectile.Center - Main.screenPosition + new Vector2(30, 0).RotatedBy(Projectile.ai[1]), t);
	}

	public void DrawBloom()
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		value = MathF.Sqrt(value);
		float colorV = 0.9f * (1 - value);
		Texture2D t = Commons.ModAsset.Trail_2_thick.Value;
		float width = 120;
		if (Projectile.timeLeft < 120)
		{
			width = Projectile.timeLeft;
		}

		Ins.Batch.Begin();
		DrawTexFlame_VFXBatch(Ins.Batch, value * 1370 * Projectile.ai[0], width * 0.2f, new Color(0, colorV * colorV * 1.6f, colorV * 12f, 0f), Projectile.Center - Main.screenPosition, t, Projectile.ai[1], (float)-Main.timeForVisualEffects * 0.32f + 1000000 + Projectile.ai[1] * 200);
		Ins.Batch.End();
	}
}
