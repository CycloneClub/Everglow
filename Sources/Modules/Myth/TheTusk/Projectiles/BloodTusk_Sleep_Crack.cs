using Everglow.Commons.DataStructures;
using Everglow.Myth.TheTusk.NPCs.BloodTusk;

namespace Everglow.Myth.TheTusk.Projectiles;

public class BloodTusk_Sleep_Crack : ModProjectile
{
	public override string Texture => ModAsset.Empty_Mod;

	public NPC Tusk;

	public override void SetDefaults()
	{
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
		Projectile.width = 100;
		Projectile.height = 100;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 10000000;
		Projectile.alpha = 255;
		Projectile.penetrate = -1;
	}

	public override void AI()
	{
		if (Tusk != null)
		{
			if (Tusk.active && Tusk.life == Tusk.lifeMax && Tusk.type == ModContent.NPCType<BloodTusk>())
			{
				Projectile.Center = Tusk.Top + new Vector2(0, -30);
				if (Projectile.ai[0] < 1)
				{
					Projectile.ai[0] = (float)Utils.Lerp(Projectile.ai[0], 1f, 0.1f);
				}
				else
				{
					Projectile.ai[0] = 1;
				}
			}
			else
			{
				if (Projectile.ai[0] > 0.1f)
				{
					Projectile.ai[0] *= 0.9f;
				}
				else
				{
					Projectile.Kill();
				}
			}
		}
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0f, 0.2f) * Projectile.ai[0]);
		Projectile.velocity *= 0;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null);
		Effect bloodCrack = ModAsset.BloodTusk_SleepShape.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		bloodCrack.Parameters["uTransform"].SetValue(model * projection);
		bloodCrack.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_flame_2.Value);
		bloodCrack.Parameters["uNoiseSize"].SetValue(1f);
		bloodCrack.Parameters["uNoiseXY"].SetValue(new Vector2(0.5f, 0.5f));
		bloodCrack.CurrentTechnique.Passes[0].Apply();
		float timeValue = (float)Main.time * 0.01f;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int y = 0; y <= 40; y++)
		{
			float coordY = y / 40f;
			float coordZ = (0.5f - coordY) * 2;
			coordZ = 1 - MathF.Pow(Math.Abs(MathF.Sin(MathHelper.Pi * coordZ / 2f)), 2.5f);
			coordZ *= 0.7f;
			AddVertex(bars, Projectile.Center + new Vector2(-coordZ * 24 * Projectile.ai[0], (y - 20) * 3f), new Vector3(timeValue, coordY, -1));
			AddVertex(bars, Projectile.Center + new Vector2(0, (y - 20) * 3f), new Vector3(0.5f + timeValue, coordY, coordZ));
		}
		Main.graphics.graphicsDevice.Textures[0] = ModAsset.BloodTusk_Sleep_HeatMap.Value;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		bars = new List<Vertex2D>();
		for (int y = 0; y <= 40; y++)
		{
			float coordY = y / 40f;
			float coordZ = (0.5f - coordY) * 2;
			coordZ = 1 - MathF.Pow(Math.Abs(MathF.Sin(MathHelper.Pi * coordZ / 2f)), 2.5f);
			coordZ *= 0.7f;
			AddVertex(bars, Projectile.Center + new Vector2(coordZ * 24 * Projectile.ai[0], (y - 20) * 3f), new Vector3(1 - timeValue, coordY, -1));
			AddVertex(bars, Projectile.Center + new Vector2(0, (y - 20) * 3f), new Vector3(0.5f - timeValue, coordY, coordZ));
		}
		Main.graphics.graphicsDevice.Textures[0] = ModAsset.BloodTusk_Sleep_HeatMap.Value;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 pos, Vector3 coords)
	{
		bars.Add(pos, new Color(1f, 1f, 1f, 0), coords);
	}
}
