using Everglow.Commons.DataStructures;

namespace Everglow.Myth.TheFirefly.Projectiles.DreamWeaver;

public class DreamWeaver_hit : ModProjectile, IWarpProjectile
{
	public override string Texture => ModAsset.DreamWeaverII_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = false;
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

	public override void PostDraw(Player player, Color lightColor)
	{
		if (Projectile.timeLeft > 200)
		{
			return;
		}
		Texture2D Shadow = ModAsset.CursedHitLight.Value;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(Shadow, Projectile.Center - Main.screenPosition, null, new Color(0.7f, 0.9f, 1, 0) * dark, 0, Shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f * dark, SpriteEffects.None, 0);
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawTexCircle(timeValue * 24 * Projectile.ai[0], 8 * (1 - timeValue) * Projectile.ai[0], new Color(0.2f * (1 - timeValue) * (1 - timeValue), 0.6f * (1 - timeValue), 1f * (1 - timeValue), 0f), Projectile.Center - Main.screenPosition, Commons.ModAsset.Trail_6.Value);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		if (Projectile.timeLeft > 200)
		{
			return false;
		}
		Texture2D shadow = ModAsset.CursedHit.Value;
		float dark = Math.Max((Projectile.timeLeft - 150) / 50f, 0);
		Main.spriteBatch.Draw(shadow, Projectile.Center - Main.screenPosition, null, Color.White * dark, 0, shadow.Size() / 2f, 2.2f * Projectile.ai[0] / 15f, SpriteEffects.None, 0);
		Texture2D light = Commons.ModAsset.StarSlash.Value;
		float timeValue = (200 - Projectile.timeLeft) / 200f;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0.2f * (1 - timeValue) * (1 - timeValue), 0.5f * (1 - timeValue), 1f, 0f), 0 + Projectile.ai[1], light.Size() / 2f, new Vector2(MathF.Pow(1 - timeValue, 2f), 1) * Projectile.ai[0] * 0.45f, SpriteEffects.None, 0);
		// Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0.2f * (1 - timeValue) * (1 - timeValue), 0.5f * (1 - timeValue), 1f, 0f), 1.57f + Projectile.ai[1], light.Size() / 2f, new Vector2(0.5f, dark) * Projectile.ai[0] * 0.35f, SpriteEffects.None, 0);
		return false;
	}

	private static void DrawTexCircle(float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();
		for (int h = 0; h <= 50; h++)
		{
			Vector2 innerPos = new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / 50f * MathHelper.TwoPi);
			Vector2 outerPos = new Vector2(0, radius).RotatedBy(h / 50f * MathHelper.TwoPi);
			innerPos.Y *= 0.5f;
			outerPos.Y *= 0.5f;
			circle.Add(center + innerPos, color, new Vector3(h / 50f, 0.8f, 0));
			circle.Add(center + outerPos, color, new Vector3(h / 50f, 0.5f, 0));
		}
		if (circle.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	private static void DrawTexCircle_VFXBatch(VFXBatch spriteBatch, float radius, float width, Color color, Vector2 center, Texture2D tex, double addRot = 0)
	{
		var circle = new List<Vertex2D>();

		Color c0 = color;
		c0.R = 0;
		for (int h = 0; h <= 50; h += 1)
		{
			c0.R = (byte)(h / radius * 2 * 255);

			Vector2 innerPos = new Vector2(0, Math.Max(radius - width, 0)).RotatedBy(h / 50f * MathHelper.TwoPi);
			Vector2 outerPos = new Vector2(0, radius).RotatedBy(h / 50f * MathHelper.TwoPi);
			innerPos.Y *= 0.5f;
			outerPos.Y *= 0.5f;

			circle.Add(center + innerPos, c0, new Vector3(h / 50f, 1, 0));
			circle.Add(center + outerPos, c0, new Vector3(h / 50f, 0, 0));
		}

		if (circle.Count > 2)
		{
			spriteBatch.Draw(tex, circle, PrimitiveType.TriangleStrip);
		}
	}

	public void DrawWarp(VFXBatch spriteBatch)
	{
		if (Projectile.timeLeft > 200)
		{
			return;
		}
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

		DrawTexCircle_VFXBatch(spriteBatch, value * 36 * Projectile.ai[0], width * 0.6f * Projectile.ai[0], new Color(colorV, colorV * 0.009f, colorV, 0f), Projectile.Center - Main.screenPosition, t, Math.PI * 0.5);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
	}
}
