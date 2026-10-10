using Microsoft.Xna.Framework;
using Terraria;
namespace Everglow.Yggdrasil.CorruptWormHive.Projectiles.Melee.TrueDeathSickle;

public class TrueDeathSickleHit : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	private float r = 20;
	private float stre2 = 1;

	public override string Texture => "Everglow/Myth/TheFirefly/Projectiles/MothBall";

	public override bool CloneNewInstances => false;

	public override bool IsCloneable => false;

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
		Projectile.extraUpdates = 2;
		Projectile.localNPCHitCooldown = 60;
		Projectile.usesLocalNPCImmunity = true;
	}

	public override void AI()
	{
		Projectile.velocity *= 0.95f;
		if (stre2 > 0)
		{
			stre2 -= 0.005f;
		}

		if (Projectile.timeLeft > 260)
		{
			r += 1f;
		}

		if (Projectile.timeLeft is <= 240 and >= 60)
		{
			r = 60 + (float)(10 * Math.Sin((Projectile.timeLeft - 60) / 60d * Math.PI));
		}

		if (Projectile.timeLeft < 60 && r > 0.5f)
		{
			r -= 1f;
		}

		Projectile.velocity *= 0;
	}

	private static void DrawTexCircle(float radious, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radious / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radious, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radious + width).RotatedBy(h / radious * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radious, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(0.5f, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious + width).RotatedBy(addRot), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D Shadow = ModAsset.TrueDeathSickleHit.Value;
		float Dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, Color.White * Dark, 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f, SpriteEffects.None, 0);
		Texture2D light = Commons.ModAsset.StarSlash.Value;
		Texture2D dark = Commons.ModAsset.StarSlash_black.Value;
		Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f), Projectile.ai[1], dark.Size() / 2f, new Vector2(0.5f * Dark * Dark, 0.72f) * Projectile.ai[0] / 3f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(1f - Dark, Dark, 1f, 0), Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f * Dark * Dark, 0.72f) * Projectile.ai[0] / 3f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(1f - Dark, Dark, 1f, 0), Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f * Dark * Dark, 0.72f) * Projectile.ai[0] / 3f, SpriteEffects.None, 0);

		return false;
	}

	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radious, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
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
		circle.Add(new Vertex2D(center + new Vector2(0, Math.Max(radious - width, 0)).RotatedBy(addRot), color, new Vector3(0, 0.8f, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radious).RotatedBy(addRot), color, new Vector3(0, 0.2f, 0)));
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

		Texture2D t = Commons.ModAsset.Trail_1.Value;
		float width = 100;
		if (Projectile.timeLeft < 180)
		{
			width = (Projectile.timeLeft - 130) * 2;
		}

		if (width <= 0)
		{
			return;
		}

		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 27 * Projectile.ai[0], width * 4, new Color(colorV, colorV * 0.02f, colorV, 0f), Projectile.Center - Main.screenPosition, t);
	}
}
