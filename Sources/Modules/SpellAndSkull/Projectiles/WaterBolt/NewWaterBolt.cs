using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Everglow.Commons.Utilities;
using Everglow.Commons.Vertex;
using Everglow.SpellAndSkull.Dusts;
using Terraria.Audio;

namespace Everglow.SpellAndSkull.Projectiles.WaterBolt;

public class NewWaterBolt : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetCustomDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.extraUpdates = 3;
		Projectile.timeLeft = 1000;
		Projectile.alpha = 0;
		Projectile.penetrate = 1;
		Projectile.scale = 1f;
		Projectile.DamageType = DamageClass.Magic;
		TrailLength = 60;

		TrailColor = new Color(0, 0.4f, 1, 0f);
		TrailWidth = 30f;
		SelfLuminous = true;
		TrailTexture = Commons.ModAsset.Trail_5.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_5_black.Value;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (TimeAfterEntityDestroy < 0)
		{
			return base.Colliding(projHitbox, targetHitbox);
		}
		float dis = (targetHitbox.Center() - Projectile.Center).Length();
		return dis < 250;
	}

	public override void Behaviors()
	{
		Projectile.velocity *= 0.9993f;
		Projectile.velocity.Y += 0.02f;

		if (Main.rand.NextBool(4))
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(-15f, -7.5f)).RotatedBy(Main.rand.NextFloat(-1.4f, 1.4f));
			float mulScale = Main.rand.NextFloat(2f, 8f);
			var blood = new WaterBoltDrop
			{
				velocity = afterVelocity / mulScale,
				Active = true,
				Visible = true,
				position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				maxTime = Main.rand.Next(32, 64),
				scale = mulScale,
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0) => base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor - timeValue * 0.2f;
		float y = 1;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}

	public override void DrawSelf()
	{
		var bars = new List<Vertex2D>();
		for (int i = 0; i < 4; ++i)
		{
			float factor = i / 3f;
			float timeValue = -(float)Main.time * 0.06f;

			Vector2 normalizedVel = Vector2.Normalize(Projectile.velocity);
			Vector2 drawPos = Projectile.Center + normalizedVel * 20f * (i - 2.4f);
			Color drawC = new Color(0f, 0.5f, 1f, 0);
			bars.Add(new Vertex2D(drawPos + normalizedVel.RotatedBy(MathHelper.PiOver2) * 40, drawC * (i / 3f), new Vector3(timeValue, 1, 1 - factor)));
			bars.Add(new Vertex2D(drawPos - normalizedVel.RotatedBy(MathHelper.PiOver2) * 40, drawC * (i / 3f), new Vector3(timeValue, 0, 1 - factor)));
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect effect = TrailShader;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.Textures[0] = TrailTexture;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public override void DestroyEntityEffect()
	{
		Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RipplingWave>(), 0, 0, Projectile.owner, 10f, 3f);
		switch (Main.rand.Next(2))
		{
			case 0:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.WaterBolt1_Mod), Projectile.Center);
				break;

			case 1:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.WaterBolt2_Mod), Projectile.Center);
				break;
		}

		for (int j = 0; j < 10; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(9, 11f), 0).RotatedByRandom(6.283) * Projectile.scale;
			int dust0 = Dust.NewDust(Projectile.Center - Projectile.velocity * 3 + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, ModContent.DustType<ShatterDrop_1>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f) * Projectile.scale * 0.4f * 5);
			Main.dust[dust0].noGravity = true;
		}
		for (int j = 0; j < 20; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(9, 11f), 0).RotatedByRandom(6.283) * Projectile.scale;
			int dust1 = Dust.NewDust(Projectile.Center - Projectile.velocity * 3 + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, ModContent.DustType<ShatterDrop_0>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(3.7f, 5.1f) * 5);
			Main.dust[dust1].alpha = (int)(Main.dust[dust1].scale * 50 / 5);
			Main.dust[dust1].rotation = Main.rand.NextFloat(0, 6.283f);
		}
	}
}
