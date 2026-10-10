namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots;

internal class KSSlingshotHit : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MiscsProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 68;
		Projectile.height = 68;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 120;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 3;
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	private Effect ef;
	private float radius = 0;

	public override void PostDraw(Color lightColor)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
		var bars = new List<Vertex2D>();
		ef = ModContent.Request<Effect>("Everglow/Myth/Effects/FadeSlingshot").Value;
		float widx = Projectile.timeLeft / 120f;
		float widxM = 1f - widx;
		radius = (float)(Math.Sqrt(5 * widxM) * 20);
		float width = widx * widx * 80f + 10;
		for (int i = 0; i < 41; ++i)
		{
			Vector2 vDp = new Vector2(0, radius).RotatedBy(i / 20d * Math.PI);
			var normalDir = Vector2.Normalize(vDp);

			var factor = i / 12.5f;
			var color = Color.Lime;
			var w = MathHelper.Lerp(1f, 0.05f, 0.5f);
			float delk0 = (width - radius) / (float)width / 2f;
			if (delk0 < 0)
			{
				delk0 = 0;
			}

			bars.Add(new Vertex2D(vDp + Projectile.Center + normalDir * width, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
			bars.Add(new Vertex2D(vDp + Projectile.Center + normalDir * -Math.Clamp(width, 0, radius), color, new Vector3((float)Math.Sqrt(factor), delk0, w)));
		}

		var triangleList = new List<Vertex2D>();

		if (bars.Count > 2)
		{
			triangleList.Add(bars[0]);
			var vertex = new Vertex2D((bars[0].position + bars[1].position) * 0.5f + Vector2.Normalize(Projectile.velocity) * 30, Color.White, new Vector3(0, 0.5f, 1));
			triangleList.Add(bars[1]);
			triangleList.Add(vertex);
			for (int i = 0; i < bars.Count - 2; i += 2)
			{
				triangleList.Add(bars[i]);
				triangleList.Add(bars[i + 2]);
				triangleList.Add(bars[i + 1]);

				triangleList.Add(bars[i + 1]);
				triangleList.Add(bars[i + 2]);
				triangleList.Add(bars[i + 3]);
			}
			RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;

			ef.Parameters["uTransform"].SetValue(model * projection);
			ef.Parameters["uTime"].SetValue(-(float)Main.time * 0.06f);
			ef.Parameters["maxr"].SetValue(widxM * widxM);
			ef.Parameters["stre"].SetValue(Projectile.ai[0]);
			Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>("Everglow/Myth/UIImages/VisualTextures/heatmapBlueRain").Value;
			Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("Everglow/Myth/UIImages/VisualTextures/FogTraceGamma").Value;
			Main.graphics.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("Everglow/Myth/UIImages/VisualTextures/FogTraceGamma").Value;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointWrap;
			ef.CurrentTechnique.Passes[0].Apply();

			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

			Main.graphics.GraphicsDevice.RasterizerState = originalState;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
	}
}
