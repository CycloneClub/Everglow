using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class AmberBall : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 5;
		Projectile.ArmorPenetration = 0;
		Projectile.friendly = true;
		Projectile.timeLeft = 900;
		Projectile.tileCollide = true;
		Projectile.penetrate = 1;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.scale = Main.rand.NextFloat(2, 2.4f);
		Projectile.magic = true;
	}

	public bool Stick = false;

	public override void AI()
	{
		Projectile.velocity.Y += 0.1f;
		Projectile.velocity *= 0.99f;
		if (Projectile.timeLeft <= 890)
		{
			foreach (Projectile projectile in Main.projectile)
			{
				if (projectile.active && projectile.type == Type)
				{
					if ((Projectile.Center - projectile.Center).Length() < (projectile.scale + Projectile.scale) * 5)
					{
						Projectile.velocity = projectile.velocity * 0.5f + Projectile.velocity * 0.5f;
						Stick = true;
					}
				}
			}
		}
		Projectile.ai[0]++;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var bars = new List<Vertex2D>();
		var barsReflect = new List<Vertex2D>();
		for (int i = 0; i < 11; i++)
		{
			Vector2 radius = new Vector2((i - 5) * Projectile.scale, 0).RotatedBy(Projectile.rotation);
			float width = MathF.Sqrt(Math.Max(0, 25 * Projectile.scale * Projectile.scale - (i - 5) * Projectile.scale * ((i - 5) * Projectile.scale)));
			Vector2 normal = new Vector2(width, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2);
			AddVertex(bars, radius + normal + Projectile.Center, new Vector3(i * 0.1f + Projectile.whoAmI * 0.17f, 1, 0));
			AddVertex(bars, radius - normal + Projectile.Center, new Vector3(i * 0.1f + Projectile.whoAmI * 0.17f, 0f, 0));

			AddVertex2(barsReflect, radius + normal + Projectile.Center, new Vector3(i * 0.1f + Projectile.whoAmI * 0.17f, 1, 0));
			AddVertex2(barsReflect, radius - normal + Projectile.Center, new Vector3(i * 0.1f + Projectile.whoAmI * 0.17f, 0f, 0));
		}

		float duration = 1f;
		if (Projectile.timeLeft < 60f)
		{
			duration = Projectile.timeLeft / 60f;
		}
		if (bars.Count > 2)
		{
			SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Effect effect = ModAsset.AmberCrystalShader.Value;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(0)) * Main.GameViewMatrix.TransformationMatrix;
			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.Parameters["duration"].SetValue(duration);
			effect.Parameters["uNoiseSize"].SetValue(3);
			effect.Parameters["uNoiseXY"].SetValue(new Vector2(0.5f, 0.5f));
			effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_structureHexagon.Value);
			effect.Parameters["uHeatMap"].SetValue(ModAsset.YggdrasilAmberLaser_crystal_heatMap.Value);
			effect.CurrentTechnique.Passes[0].Apply();

			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_11.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.Parameters["duration"].SetValue(duration);
			effect.Parameters["uNoiseSize"].SetValue(12);
			effect.Parameters["uNoiseXY"].SetValue(Main.screenPosition * 0.00006f);
			effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_lava.Value);
			effect.Parameters["uHeatMap"].SetValue(ModAsset.YggdrasilAmberLaser_crystal_light_heatMap.Value);
			effect.CurrentTechnique.Passes[0].Apply();
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_11.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, barsReflect.ToArray(), 0, barsReflect.Count - 2);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.EmptyCrystal_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.EmptyCrystal.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		for (int x = 0; x < 6; x++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 6f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new AmberFlameDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(27, 35),
				Scale = Main.rand.NextFloat(5.20f, 16.35f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int x = 0; x < 4; x++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 8f)).RotatedByRandom(MathHelper.TwoPi);
			var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<YggdrasilAmberFlame>());
			dust.velocity = newVelocity;
			dust.noGravity = true;
		}
		for (int x = 0; x < 6; x++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 8f)).RotatedByRandom(MathHelper.TwoPi);
			var dust = Dust.NewDustDirect(Projectile.Center, 0, 0, ModContent.DustType<YggdrasilAmberMoon>());
			dust.velocity = newVelocity;
			dust.scale = Main.rand.NextFloat(1.2f, 2f);
		}
		for (int x = 0; x < 8; x++)
		{
			var d = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, ModContent.DustType<YggdrasilAmber_crack>());
			d.velocity = new Vector2(0, Main.rand.NextFloat(0, 5.5f)).RotatedByRandom(MathHelper.TwoPi);
			d.position += new Vector2(0, Main.rand.NextFloat(0f, 20f)).RotatedByRandom(MathHelper.TwoPi);
			d.scale *= 3f;
		}
		SoundEngine.PlaySound(SoundID.NPCDeath1.WithPitchOffset(1), Projectile.Center);
		base.OnKill(timeLeft);
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 position, Vector3 texCoord)
	{
		float duration = 1f;
		if (Projectile.timeLeft < 60f)
		{
			duration = Projectile.timeLeft / 60f;
		}
		float value = 0f;
		value = (60 - Projectile.ai[0]) / 60f;
		Color color = Color.Lerp(Lighting.GetColor(position.ToTileCoordinates()) * 0.6f, new Color(1f, 1f, 0.4f, 0.9f), value) * duration;
		bars.Add(position - Main.screenPosition, color, texCoord);
	}

	public void AddVertex2(List<Vertex2D> bars, Vector2 position, Vector3 texCoord)
	{
		bars.Add(position - Main.screenPosition, Lighting.GetColor(position.ToTileCoordinates()) * 1.5f, texCoord);
	}
}
