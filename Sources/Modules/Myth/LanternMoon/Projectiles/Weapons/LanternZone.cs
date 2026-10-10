using Everglow.Commons.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class LanternZone : ModProjectile
{
	public int Timer = 0;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 600;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = 1;
		Projectile.aiStyle = -1;
	}

	public override void AI()
	{
		Timer++;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var drawPos = Projectile.Center;
		var drawColor = new Color(1f, 1f, 1f, 0);
		var drawColor2 = new Color(0.6f, 0.6f, 0.6f, 0);
		float disValue = 30;
		float fade = 1f;
		if (Projectile.timeLeft < 60)
		{
			fade *= Projectile.timeLeft / 60f;
		}
		float fade2 = 1f;
		if (Projectile.timeLeft < 120)
		{
			fade2 *= Projectile.timeLeft / 120f;
		}
		fade2 = MathF.Pow(fade2, 0.5f);
		float colorFade = 1f;
		if (Timer < 30)
		{
			colorFade *= Timer / 30f;
		}
		if (Projectile.timeLeft < 30)
		{
			colorFade *= Projectile.timeLeft / 30f;
		}
		List<Vertex2D> bars = new List<Vertex2D>();
		List<Vertex2D> bars_dark = new List<Vertex2D>();
		List<Vertex2D> bars_bloom = new List<Vertex2D>();
		for (int i = 0; i <= 80; i++)
		{
			Vector2 radius = new Vector2(0, disValue).RotatedBy(i / 80f * MathHelper.TwoPi + 0);
			Vector2 radius_out = radius * 3f;
			float xCoord = i / 80f * 8;
			bars_dark.Add(drawPos + radius, Color.White * colorFade, new Vector3(xCoord, 1, fade));
			bars_dark.Add(drawPos + radius_out, Color.White * colorFade, new Vector3(xCoord, 0, fade));
			bars.Add(drawPos + radius, drawColor2 * colorFade, new Vector3(xCoord, 1, fade));
			bars.Add(drawPos + radius_out, drawColor * colorFade, new Vector3(xCoord, 0, fade));
			bars_bloom.Add(drawPos + radius, drawColor2 * colorFade, new Vector3(xCoord, 1, fade));
			bars_bloom.Add(drawPos + radius_out, drawColor * colorFade, new Vector3(xCoord, 0, fade));
			Lighting.AddLight(drawPos + radius * 2f, new Vector3(0.7f, 0.01f, 0.03f) * fade);
		}

		var darkColor = new Color(0.75f, 0f, 0.12f, 0) * 0.75f;
		List<Vertex2D> bars_shadowRing = new List<Vertex2D>();
		for (int i = 0; i <= 60; i++)
		{
			Vector2 radius = new Vector2(0, disValue * 2f).RotatedBy(i / 60f * MathHelper.TwoPi + 0);
			Vector2 radius_out = new Vector2(0, disValue * 2.3f).RotatedBy(i / 60f * MathHelper.TwoPi + 0);
			float xCoord = i / 60f * 4;
			bars_shadowRing.Add(drawPos + radius, darkColor * colorFade, new Vector3(xCoord + Timer * 0.0009f + Projectile.whoAmI * 0.35f, 1, fade2));
			bars_shadowRing.Add(drawPos + radius_out * 1.1f, darkColor, new Vector3(xCoord + Timer * 0.0009f + Projectile.whoAmI * 0.35f, 0, fade2));
		}
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect effect0 = ModAsset.WizardLantern_Thunder_Matrix_Shader.Value;
		effect0.Parameters["uTransform"].SetValue(model * projection);
		effect0.Parameters["size1"].SetValue(Vector2.One);
		effect0.CurrentTechnique.Passes[0].Apply();

		if (bars_dark.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_perlin.Value;
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.LanternZone_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_dark.ToArray(), 0, bars_dark.Count - 2);
		}

		if (bars.Count > 0)
		{
			var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
			Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_perlin.Value;
			Main.graphics.GraphicsDevice.Textures[0] = texture;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		if (bars_bloom.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_perlin.Value;
			Main.graphics.GraphicsDevice.Textures[0] = ModAsset.LanternZone_bloom.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_bloom.ToArray(), 0, bars_bloom.Count - 2);
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_bloom.ToArray(), 0, bars_bloom.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect effect1 = ModAsset.WizardLantern_CurseDarkPart_Matrix_Shader.Value;
		effect1.Parameters["uTransform"].SetValue(model * projection);
		effect1.Parameters["size1"].SetValue(Vector2.One * 0.33333f);
		effect1.Parameters["size2"].SetValue(Vector2.One * 0.33333f);
		effect1.Parameters["warpSize"].SetValue(Vector2.One * 0.56f);
		effect1.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.003f);
		effect1.CurrentTechnique.Passes[0].Apply();

		Main.graphics.GraphicsDevice.Textures[2] = Commons.ModAsset.Noise_Sand_shallow.Value;
		Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_rgb_large.Value;
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_9.Value;
		if (bars_shadowRing.Count > 0)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_shadowRing.ToArray(), 0, bars_shadowRing.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}
}
