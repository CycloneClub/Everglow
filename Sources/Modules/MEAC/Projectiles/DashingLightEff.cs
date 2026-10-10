using Everglow.Commons.MEAC;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;

namespace Everglow.MEAC.Projectiles;

public class DashingLightEff : ModProjectile, IWarpProjectile
{
	public override string Texture => "Terraria/Images/Projectile_0";

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.OverPlayers;
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.timeLeft = 40;
		Projectile.scale = 1f;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.extraUpdates = 2;
		Projectile.DamageType = DamageClass.Melee;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 1000;
		oldPos = new Vector2[25];
	}

	private Vector2[] oldPos = new Vector2[25];
	private Vector2 vec = Vector2.Zero;

	public override void AI()
	{
		Lighting.AddLight(Projectile.Center + Projectile.velocity * (40 - Projectile.timeLeft) * 0.6f, 0.9f, 0.6f, 0);
		Player player = Main.player[Projectile.owner];
		if (Projectile.ai[0] == 0)
		{
			if (Projectile.timeLeft > 20)
			{
				vec = player.Center + (40f - Projectile.timeLeft) * Projectile.velocity * 0.2f;
			}

			Projectile.Center = vec;
		}
		else
		{
			vec = Projectile.Center;
			Projectile.friendly = true;
		}
		if (Projectile.timeLeft > 20)
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			for (int i = oldPos.Length - 1; i > 0; --i)
			{
				oldPos[i] = oldPos[i - 1];
			}

			oldPos[0] = (40 - Projectile.timeLeft) * Projectile.velocity;
		}
		if (Projectile.timeLeft < 20)
		{
			Projectile.velocity = Vector2.Zero;
			Projectile.friendly = false;
		}
	}


	public void DrawWarp(VFXBatch spriteBatch)
	{
		float timeValue = 1f;
		if (Projectile.timeLeft < 60)
		{
			timeValue = Projectile.timeLeft / 60f;
		}
		Vector2 normalized = Vector2.Normalize(Projectile.velocity.RotatedBy(Math.PI * 0.5)) * 30f * timeValue;
		Vector2 start = Projectile.Center + Vector2.Normalize(Projectile.velocity) * (100 - Projectile.timeLeft) * 3;
		Vector2 end = Projectile.Center - Vector2.Normalize(Projectile.velocity) * 100;

		Vector2 middle = Vector2.Lerp(end, start, 0.6f);
		float time = (float)(Main.time * 0.03);
		Color alphaColor = Color.White;
		alphaColor.A = 0;
		alphaColor.R = (byte)(((Projectile.rotation + MathHelper.TwoPi + MathHelper.PiOver4 * 4) % MathHelper.TwoPi) / MathHelper.TwoPi * 255);
		alphaColor.G = (byte)(Projectile.timeLeft * 0.3f);

		Color alphaColor2 = alphaColor;
		alphaColor2.G = 0;

		List<Vertex2D> bars = new List<Vertex2D>
		{
			new Vertex2D(start - Main.screenPosition, new Color(alphaColor.R / 255f, 0.05f, 0, 0), new Vector3(1 + time, 0, 0)),
			new Vertex2D(start - Main.screenPosition, new Color(alphaColor.R / 255f, 0.05f, 0, 0), new Vector3(1 + time, 1, 0)),
			new Vertex2D(middle + normalized - Main.screenPosition, alphaColor, new Vector3(0.5f + time, 0, 0.5f)),
			new Vertex2D(middle - normalized - Main.screenPosition, alphaColor, new Vector3(0.5f + time, 1, 0.5f)),
			new Vertex2D(end + normalized - Main.screenPosition, alphaColor2, new Vector3(0f + time, 0, 1)),
			new Vertex2D(end - normalized - Main.screenPosition, alphaColor2, new Vector3(0f + time, 1, 1)),
		};
		spriteBatch.Draw(Commons.ModAsset.Noise_spiderNet.Value, bars, PrimitiveType.TriangleStrip);
	}

	public int collisionTimer = 0;

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Vector2 drawCenter = Projectile.Center + Vector2.Normalize(Projectile.velocity) * (100 - Projectile.timeLeft) * 3;
		float timeValue = 1f;
		if (Projectile.timeLeft < 30)
		{
			timeValue = Projectile.timeLeft / 30f;
		}
		Vector2 vel = Vector2.Normalize(Projectile.velocity);
		Vector2 width = vel.RotatedBy(MathF.PI * 0.5) * 150 * timeValue;
		Color drawColor = new Color(1f, 0.6f, 0.3f, 0);
		int trailLength = (50 - Projectile.timeLeft) * 3 / 5;

		float timeEffectValue = (float)(Main.time * 0.06f) + Projectile.whoAmI * 0.27f;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int x = 0; x < trailLength; x++)
		{
			float value = 1;
			if (x > trailLength - 37)
			{
				value = (trailLength - 1 - x) / 36f;
			}
			bars.Add(drawCenter - vel * 17 * x + width, drawColor * value, new Vector3(x / 1000f - timeEffectValue, 1, MathF.Sin(x / 32f)));
			bars.Add(drawCenter - vel * 17 * x - width, drawColor * value, new Vector3(x / 1000f - timeEffectValue, 0, MathF.Sin(x / 32f)));
		}

		List<Vertex2D> barsDark = new List<Vertex2D>();
		for (int x = 0; x < trailLength; x++)
		{
			float value = 1;
			if (x > trailLength - 37)
			{
				value = (trailLength - 1 - x) / 36f;
			}
			barsDark.Add(drawCenter - vel * 17 * x + width, Color.White * value, new Vector3(x / 1000f - timeEffectValue, 1, MathF.Sin(x / 32f)));
			barsDark.Add(drawCenter - vel * 17 * x - width, Color.White * value, new Vector3(x / 1000f - timeEffectValue, 0, MathF.Sin(x / 32f)));
		}

		List<Vertex2D> bars2 = new List<Vertex2D>();
		for (int x = 0; x < trailLength; x++)
		{
			float value = 1;
			if (x > trailLength - 20)
			{
				value = (trailLength - 1 - x) / 19f;
			}
			bars2.Add(drawCenter - vel * 20 * x + width * 0.16f, drawColor * value * 2, new Vector3(x / 24f - timeEffectValue * 1.2f, 1f, MathF.Sin(x / 8f)));
			bars2.Add(drawCenter - vel * 20 * x - width * 0.16f, drawColor * value * 2, new Vector3(x / 24f - timeEffectValue * 1.2f, 0f, MathF.Sin(x / 8f)));
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
		Effect effect = Commons.ModAsset.StabSwordEffect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uProcession"].SetValue(0.5f);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_black_thick.Value;
		Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsDark.ToArray(), 0, barsDark.Count - 2);

		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_0.Value;
		Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);

		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_2.Value;
		Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		return false;
	}
}
