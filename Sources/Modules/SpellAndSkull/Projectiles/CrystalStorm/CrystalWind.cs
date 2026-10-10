using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;

namespace Everglow.SpellAndSkull.Projectiles.CrystalStorm;

public class CrystalWind : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 3;
		Projectile.timeLeft = 120;
		Projectile.alpha = 0;
		Projectile.penetrate = 1;
		Projectile.scale = 1f;

		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
	}

	private Vector2 aimCenter = Vector2.Zero;
	private Vector2 oldAimCenter = Vector2.Zero;

	public override void AI()
	{
		if ((oldAimCenter - Main.projectile[(int)Projectile.ai[0]].Center).Length() > 200 && oldAimCenter != Vector2.Zero)
		{
			if (Projectile.timeLeft > 20)
			{
				Projectile.timeLeft -= 5;
			}
		}
		aimCenter = Main.projectile[(int)Projectile.ai[0]].Center;
		oldAimCenter = Main.projectile[(int)Projectile.ai[0]].Center;

		float Dy = aimCenter.Y - Projectile.position.Y;
		float xCoefficient = Dy * Dy / 600f - 0.4f * Dy + 50;
		Vector2 TrueAim = aimCenter + new Vector2(xCoefficient * (float)Math.Sin(Main.timeForVisualEffects * 0.1 * Projectile.extraUpdates + Projectile.rotation), 0) - Projectile.Center;

		Projectile.alpha = (byte)(Projectile.alpha * 0.95 + xCoefficient * 0.05);

		Projectile.velocity = Projectile.velocity * 0.75f + new Vector2(TrueAim.SafeNormalize(new Vector2(0, 0.05f)).X, -Projectile.ai[1] * 0.3f) * 0.25f / Projectile.alpha * 500f;
		Projectile.velocity *= Main.rand.NextFloat(0.85f, 1.15f);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float timeValue = (120 - Projectile.timeLeft) / 120f;

		var bars = new List<Vertex2D>();
		float width = 48;
		if (Projectile.timeLeft <= 40)
		{
			width *= Projectile.timeLeft / 40f;
		}

		int trueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			trueL++;
		}
		for (int i = 1; i < trueL; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)trueL;
			var c0 = Color.Lerp(new Color(75, 0, 105, 0), new Color(0, 120, 255, 0), factor);
			c0 *= timeValue;
			float x0 = factor * 0.6f - (float)(Main.timeForVisualEffects / 35d) + 10000;
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c0, new Vector3(x0, 1, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c0, new Vector3(x0, 0, 0)));
		}
		Texture2D t = Commons.ModAsset.Trail_3.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		return false;
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		var bars = new List<Vertex2D>();
		float width = 48;

		int trueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			trueL++;
		}
		for (int i = 1; i < trueL; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)trueL;
			var w = MathHelper.Lerp(1f, 0.05f, factor);
			float x0 = factor * 0.6f - (float)(Main.timeForVisualEffects / 35d) + 10000;
			var c0 = new Color(normalDir.ToRotation() / 6.283f, 0.06f, 0f);
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c0, new Vector3(x0, 1, w)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(5f, 5f) - Main.screenPosition, c0, new Vector3(x0, 0, w)));
		}
		Texture2D t = Commons.ModAsset.Trail_3.Value;
		if (bars.Count > 3)
		{
			spriteBatch.Draw(t, bars, PrimitiveType.TriangleStrip);
		}
	}
}
