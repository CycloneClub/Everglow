using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;

namespace Everglow.Commons.Templates.Weapons.Slingshots;

/// <summary>
/// ai[0]强度ai[1]角度
/// </summary>
public abstract class SlingshotHitProjectile : ModProjectile, IWarpProjectile
{
	public override bool CloneNewInstances => false;

	public override bool IsCloneable => false;

	public override void SetDefaults()
	{
		Projectile.width = 120;
		Projectile.height = 120;
		Projectile.aiStyle = -1;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 200;
		Projectile.extraUpdates = 9;
		Projectile.DamageType = DamageClass.Ranged;
		SetDef();
	}

	public virtual void SetDef()
	{
	}

	private static void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(1, 0, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(addRot), color, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 0, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		float value = (200 - Projectile.timeLeft) / 200f;
		float colorV = 0.02f * MathF.Sqrt(Projectile.ai[0]) * (1 - value);
		Texture2D t = ModAsset.Trail_1.Value;
		DrawTexCircle(value * 22 * MathF.Sqrt(Projectile.ai[0]), 8 * MathF.Sqrt(Projectile.ai[0]) * value, new Color(colorV, colorV, colorV, 0f), Projectile.Center - Main.screenPosition, t);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		for (int h = 0; h < radius / 2; h += 1)
		{
			float colorR = (h / radius * MathF.PI * 4 + (float)addRot + MathF.PI * 1.5f) % (MathF.PI * 2f) / (MathF.PI * 2f);
			float color2R = ((h + 1) / radius * MathF.PI * 4 + (float)addRot + MathF.PI * 1.5f) % (MathF.PI * 2f) / (MathF.PI * 2f);

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
		float colorV = 0.6f * MathF.Sqrt(Projectile.ai[0]) * (1 - value);
		Texture2D t = ModAsset.Trail_1.Value;
		DrawTexCircle_VFXBatch(spriteBatch, value * 22 * MathF.Sqrt(Projectile.ai[0]), 8 * MathF.Sqrt(Projectile.ai[0]) * value, new Color(colorV, colorV * 0.04f * (1 - value), colorV, 0f), Projectile.Center - Main.screenPosition, t);
	}
}
