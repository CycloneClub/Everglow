using Everglow.Commons.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon.FaelanternProj;

public class FaeCharmRing : ModProjectile, IWarpProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	public override string Texture => ModAsset.Fae_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 120;
		Projectile.height = 120;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 200;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 6;
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		bool bool0 = (targetHitbox.TopLeft() - projHitbox.Center()).Length() < 150;
		bool bool1 = (targetHitbox.TopRight() - projHitbox.Center()).Length() < 150;
		bool bool2 = (targetHitbox.BottomLeft() - projHitbox.Center()).Length() < 150;
		bool bool3 = (targetHitbox.BottomRight() - projHitbox.Center()).Length() < 150;
		return bool0 || bool1 || bool2 || bool3;
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
		var light = lightColor.ToVector4();
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		DrawTexCircle(MathF.Sqrt(timeValue) * 24 * Projectile.ai[0], 8 * (1 - timeValue) * Projectile.ai[0], lightColor * (1 - timeValue) * 0.75f, Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_black.Value);
		DrawTexCircle(MathF.Sqrt(timeValue) * 24 * Projectile.ai[0], 8 * (1 - timeValue) * Projectile.ai[0], new Color(0.13f * light.X, 0.976f * light.Y, 0.977f * light.Z, 0f) * (1 - timeValue), Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_6.Value);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
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

		DrawTexCircle_VFXBatch(spriteBatch, MathF.Sqrt(value) * 34 * Projectile.ai[0], width * 2, new Color(colorV, colorV * 0.06f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}
}
