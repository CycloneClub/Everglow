using Everglow.Commons.DataStructures;
using Everglow.Myth.TheFirefly.Projectiles.DreamWeaver;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class DreamWeaverBall : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 3000000;
		Projectile.alpha = 0;
		Projectile.penetrate = 78;
		Projectile.scale = 0f;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 24;
		Projectile.DamageType = DamageClass.Magic;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
	}

	public float BallPower = 0;
	public float PosLerpValue = 0;
	public Vector2 HitPoint = Vector2.Zero;

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Lighting.AddLight(Projectile.Center, 0, 0.2f * Projectile.scale, 1.4f * Projectile.scale);
		Projectile.velocity *= 0.99f;
		Projectile.scale = (float)Utils.Lerp(Projectile.scale, 1f, 0.01);
		Projectile.timeLeft -= player.ownedProjectileCounts[Projectile.type];
		if (HitPoint == Vector2.zeroVector)
		{
			HitPoint = player.Center;
		}
		if (PosLerpValue < 0.1f)
		{
			PosLerpValue += 0.0005f;
		}
		Projectile.Center = Vector2.Lerp(Projectile.Center, HitPoint + new Vector2(0, -300), PosLerpValue);

		if (player.HeldItem.type == ModContent.ItemType<Myth.TheFirefly.Items.Weapons.DreamWeaver>())
		{
			if (BallPower < 1.101)
			{
				BallPower += 0.005f;
			}
		}
		else
		{
			BallPower -= 0.06f;
			if (BallPower < -1)
			{
				Projectile.Kill();
			}
		}
		Projectile.ai[0] += 1;
		if (Projectile.ai[0] > 9 - BallPower * 5)
		{
			Projectile.ai[0] = 0;

			float timeValue = (float)(Main.time * 0.008f);
			float mulSize = 1f + MathF.Sin(timeValue * 5f + Projectile.whoAmI) * 0.05f;
			float radius = 30 * mulSize * Projectile.scale;
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, radius).RotatedBy(Main.rand.NextFloat(-1.4f, 1.4f)), new Vector2(0, 20), ModContent.ProjectileType<DreamWeaver_Rain>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner);
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		BallPower = -1;
		Projectile.ai[0] = 0;
		Projectile.scale = 0;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float timeValue = (float)(Main.time * 0.008f);
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect sphere = ModAsset.SpherePerspective_DreamWeaverBall.Value;
		Effect antiSphere = ModAsset.AntiSpherePerspective_DreamWeaverBall.Value;
		List<Vertex2D> triangleList = new List<Vertex2D>();

		float mulSize = 1f + MathF.Sin(timeValue * 5f + Projectile.whoAmI) * 0.05f;
		float radius = 120 * mulSize * Projectile.scale;
		var baseColor = Color.Black;

		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, radius), baseColor, new Vector3(-1, 1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, -radius), baseColor, new Vector3(-1, -1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, -radius), baseColor, new Vector3(1, -1, timeValue)));

		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, radius), baseColor, new Vector3(-1, 1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, -radius), baseColor, new Vector3(1, -1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, radius), baseColor, new Vector3(1, 1, timeValue)));

		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

		antiSphere.Parameters["uTransform"].SetValue(model * projection);
		antiSphere.Parameters["circleCenter"].SetValue(new Vector3(0, 0, -2));
		antiSphere.Parameters["radiusOfCircle"].SetValue(1f);
		antiSphere.Parameters["uTime"].SetValue(timeValue * 1.24f);
		antiSphere.Parameters["uPower"].SetValue(BallPower);

		antiSphere.CurrentTechnique.Passes[0].Apply();

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_black_thick.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_rgb.Value;
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		mulSize = 1f + MathF.Sin(timeValue * 15f + Projectile.whoAmI) * 0.07f;
		radius = 80 * mulSize * Projectile.scale;
		baseColor = new Color(0.05f, 0.35f, 1, 0);

		triangleList = new List<Vertex2D>();
		for (int i = 0; i <= 120; i++)
		{
			Vector2 drawPoint = new Vector2(0, radius).RotatedBy(i / 120f * MathHelper.TwoPi);
			Color drawColor = baseColor;
			if (drawPoint.Y / radius > BallPower)
			{
				drawColor *= 0;
			}
			drawPoint.Y *= -1;
			drawPoint *= 0.57f;
			triangleList.Add(drawPoint * 1.2f - Main.screenPosition + Projectile.Center, drawColor, new Vector3(0, 1, 0));
			triangleList.Add(drawPoint - Main.screenPosition + Projectile.Center, drawColor, new Vector3(0, 0.5f, 0));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, triangleList.ToArray(), 0, triangleList.Count - 2);

		Texture2D Light = SpellAndSkull.ModAsset.MagnetSphereII.Value;
		Texture2D Shade = SpellAndSkull.ModAsset.NewWaterBoltShade.Value;
		Main.spriteBatch.Draw(Shade, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Shade.Size() / 2f, 1.08f * Projectile.scale * MathF.Max(BallPower, 0), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition, null, baseColor, Projectile.rotation, Light.Size() / 2f, 0.8f * Projectile.scale * MathF.Max(BallPower, 0), SpriteEffects.None, 0);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		triangleList = new List<Vertex2D>();

		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, radius), baseColor, new Vector3(-1, 1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, -radius), baseColor, new Vector3(-1, -1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, -radius), baseColor, new Vector3(1, -1, timeValue)));

		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, radius), baseColor, new Vector3(-1, 1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, -radius), baseColor, new Vector3(1, -1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, radius), baseColor, new Vector3(1, 1, timeValue)));

		antiSphere.Parameters["uTransform"].SetValue(model * projection);
		antiSphere.Parameters["circleCenter"].SetValue(new Vector3(0, 0, -2));
		antiSphere.Parameters["radiusOfCircle"].SetValue(1f);
		antiSphere.Parameters["uTime"].SetValue(timeValue * 0.4f);
		antiSphere.Parameters["uPower"].SetValue(BallPower);

		antiSphere.CurrentTechnique.Passes[0].Apply();

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_hiveCyber.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_SolarSpectrum.Value;
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, radius), baseColor, new Vector3(-1, 1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, -radius), baseColor, new Vector3(-1, -1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, -radius), baseColor, new Vector3(1, -1, timeValue)));

		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, radius), baseColor, new Vector3(-1, 1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, -radius), baseColor, new Vector3(1, -1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, radius), baseColor, new Vector3(1, 1, timeValue)));

		sphere.Parameters["uTransform"].SetValue(model * projection);
		sphere.Parameters["circleCenter"].SetValue(new Vector3(0, 0, -2));
		sphere.Parameters["radiusOfCircle"].SetValue(1f);
		sphere.Parameters["uTime"].SetValue(timeValue * 0.4f);
		sphere.Parameters["uPower"].SetValue(BallPower);

		sphere.CurrentTechnique.Passes[0].Apply();

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_hiveCyber.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_SolarSpectrum.Value;
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		triangleList = new List<Vertex2D>();
		mulSize = 1f + MathF.Sin(timeValue * 5f + Projectile.whoAmI) * 0.05f;
		radius = 120 * mulSize * Projectile.scale;
		baseColor = Color.Black;

		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, radius), baseColor, new Vector3(-1, 1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, -radius), baseColor, new Vector3(-1, -1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, -radius), baseColor, new Vector3(1, -1, timeValue)));

		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(radius, radius), baseColor, new Vector3(-1, 1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, -radius), baseColor, new Vector3(1, -1, timeValue)));
		triangleList.Add(new Vertex2D(Projectile.Center - new Vector2(-radius, radius), baseColor, new Vector3(1, 1, timeValue)));

		sphere.Parameters["uTransform"].SetValue(model * projection);
		sphere.Parameters["circleCenter"].SetValue(new Vector3(0, 0, -2));
		sphere.Parameters["radiusOfCircle"].SetValue(1f);
		sphere.Parameters["uTime"].SetValue(timeValue * 1.24f);
		sphere.Parameters["uPower"].SetValue(BallPower);

		sphere.CurrentTechnique.Passes[0].Apply();

		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_black_thick.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_rgb.Value;
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		ScreenShaker Gsplayer = Main.player[Projectile.owner].GetModPlayer<ScreenShaker>();
		Gsplayer.FlyCamPosition = new Vector2(0, 22).RotatedByRandom(6.283);
		SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
		for (int d = 0; d < 14; d++)
		{
			Vector2 BasePos = Projectile.Center - new Vector2(4) + new Vector2(0, 40);
			var d0 = Dust.NewDustDirect(BasePos, 0, 0, DustID.Electric, 0, 0, 0, default, 0.6f);
			d0.velocity = new Vector2(0, Main.rand.NextFloat(1.65f, 5.5f)).RotatedByRandom(6.283) * 3;
		}
	}

	public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
	{
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Projectile.velocity.X != oldVelocity.X)
		{
			Projectile.velocity.X = -oldVelocity.X;
		}

		if (Projectile.velocity.Y != oldVelocity.Y)
		{
			Projectile.velocity.Y = -oldVelocity.Y;
		}

		Projectile.velocity *= 0.98f;
		Projectile.penetrate -= 5;
		if (Projectile.penetrate < 0)
		{
			Projectile.Kill();
		}

		return false;
	}
}
