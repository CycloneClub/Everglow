using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Terraria.Audio;

namespace Everglow.SpellAndSkull.Projectiles.RazorbladeTyphoon;

public class HurricaneMask : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override bool CloneNewInstances => false;

	public override bool IsCloneable => false;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.extraUpdates = 3;
		Projectile.tileCollide = false;
		Projectile.DamageType = DamageClass.Magic;
	}

	public override void AI()
	{
		if (Projectile.timeLeft == 1)
		{
			var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BlackHole.BlackHole>(), Projectile.damage * 12, 0, Projectile.owner, Projectile.ai[0] * 2);
			p.CritChance = Projectile.CritChance;
			p.timeLeft = 100 + (int)(Projectile.ai[0] * 240);
			if (Projectile.ai[0] < 0.5f)
			{
				SoundEngine.PlaySound(new SoundStyle("Everglow/SpellAndSkull/Sounds/TyphoonBlackHoleWeak").WithVolumeScale(Projectile.ai[0] * 2), Projectile.Center);
			}
			else
			{
				SoundEngine.PlaySound(new SoundStyle("Everglow/SpellAndSkull/Sounds/TyphoonBlackHoleStrong").WithVolumeScale(Projectile.ai[0]), Projectile.Center);
			}
			Projectile.Kill();
		}
		if (Projectile.timeLeft < 150)
		{
			Projectile.extraUpdates = 4;
		}

		if (Projectile.timeLeft < 100)
		{
			Projectile.extraUpdates = 9;
		}

		if (Projectile.timeLeft == 100)
		{
			SoundEngine.PlaySound(new SoundStyle("Everglow/SpellAndSkull/Sounds/TyphoonBlackHoleSummon").WithVolumeScale(Projectile.ai[0]).WithPitchOffset(0f), Projectile.Center);
		}
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D Shadow = ModAsset.CursedHit.Value;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, Color.White * dark, 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] * 6f * dark, SpriteEffects.None, 0);
		Shadow = ModAsset.CursedHitLight.Value;
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, new Color(0, 225, 255, 0) * dark, 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] * 8f * dark, SpriteEffects.None, 0);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D Shadow = ModAsset.CursedHit.Value;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, Color.White * dark, 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f, SpriteEffects.None, 0);
		Texture2D light = ModAsset.CursedHitStar.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0, 225, 255, 0), 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(1f, dark * dark) * Projectile.ai[0] * 6f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0, 225, 255, 0), 1.57f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] * 6f, SpriteEffects.None, 0);
		return false;
	}

	private static void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h < radius / 2; h++)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(1, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(1, 0, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(0, 0, 0)));
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		for (int h = 0; h < radius / 2; h += 5)
		{
			circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 1, 0)));
			circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(h / radius * Math.PI * 4 + addRot), color, new Vector3(h * 2 / radius, 0, 0)));
		}
		circle.Add(new Vertex2D(center + new Vector2(0, radius).RotatedBy(addRot), color, new Vector3(0.5f, 1, 0)));
		circle.Add(new Vertex2D(center + new Vector2(0, radius + width).RotatedBy(addRot), color, new Vector3(0.5f, 0, 0)));
		if (circle.Count > 0)
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

		Texture2D t = Commons.ModAsset.Wave.Value;
		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 1200 * Projectile.ai[0], 100, new Color(colorV, colorV * 0.1f, colorV, 0f), Projectile.Center - Main.screenPosition, t);
	}
}
