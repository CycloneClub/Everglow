using Everglow.Commons.DataStructures;
using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Projectiles;

/// <summary>
/// Projectile.ai[0] is the length of tusk over gum.
/// </summary>
public class BloodFountain : ModProjectile
{
	public override string Texture => ModAsset.BloodFountain_Heatmap_Mod;

	public override void SetDefaults()
	{
		Projectile.drawLayer = ProjectileDrawLayerID.OverWiresUI;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
		Projectile.tileCollide = false;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.timeLeft = 300;
		Projectile.penetrate = -1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.timeLeft = (int)Projectile.ai[0];
		Projectile.velocity *= 0;
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		float halfMaxTime = Projectile.ai[0] * 0.5f;
		float decreaseZ = halfMaxTime - Math.Abs(Projectile.timeLeft - halfMaxTime);
		decreaseZ = Math.Clamp(decreaseZ, 0, 100) / 100f;
		decreaseZ = MathF.Pow(decreaseZ * 1.3f, 0.7f) - 0.3f;
		decreaseZ = Math.Max(decreaseZ, 0);
		foreach (var player in Main.player)
		{
			if (player.active)
			{
				if (Rectangle.Intersect(player.Hitbox, new Rectangle((int)Projectile.Center.X - 15, (int)(Projectile.Center.Y - 576 * Projectile.scale), 30, (int)(576 * Projectile.scale))) != Rectangle.emptyRectangle)
				{
					float power = (Projectile.scale * 400 - Projectile.Center.Y + player.Center.Y) / 700f;
					power = MathF.Max(power, 0);
					for (int g = 0; g < 4 * decreaseZ * power; g++)
					{
						var blood = new BloodDrop
						{
							Velocity = (new Vector2(0, 10).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.6f, 1.5f) + new Vector2((player.Center.X - Projectile.Center.X) * 0.2f, 4 * power)) * decreaseZ * power,
							Active = true,
							Visible = true,
							Position = player.Center + new Vector2(4, -34),
							MaxTime = Main.rand.Next(54, 360) * decreaseZ,
							Scale = Main.rand.NextFloat(6f, 15f) * decreaseZ,
							Rotation = Main.rand.NextFloat(6.283f),
							ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
						};
						Ins.VFXManager.Add(blood);
					}
					for (int g = 0; g < 4 * decreaseZ * power; g++)
					{
						var bloodSplash = new BloodSplash
						{
							Velocity = (new Vector2(0, 10).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.6f, 1.5f) + new Vector2((player.Center.X - Projectile.Center.X) * 0.2f, 4 * power)) * decreaseZ * power,
							Active = true,
							Visible = true,
							Position = player.Center + new Vector2(4, -34),
							MaxTime = Main.rand.Next(54, 75) * decreaseZ,
							Scale = Main.rand.NextFloat(6f, 18f) * decreaseZ,
							ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
						};
						Ins.VFXManager.Add(bloodSplash);
					}
				}
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return Rectangle.Intersect(targetHitbox, new Rectangle((int)Projectile.Center.X - 15, (int)(Projectile.Center.Y - 576 * Projectile.scale), 30, (int)(576 * Projectile.scale))) != Rectangle.emptyRectangle;
	}


	public override bool PreDraw(Player player, ref Color lightColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null);

		Effect dissolve = ModAsset.BloodFountain_Shader.Value;

		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_forceField_medium.Value);
		dissolve.Parameters["uNoiseSize"].SetValue(0.7f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();

		float halfMaxTime = Projectile.ai[0] * 0.5f;
		float decreaseZ = halfMaxTime - Math.Abs(Projectile.timeLeft - halfMaxTime);
		decreaseZ = Math.Clamp(decreaseZ, 0, 100) / 100f;
		decreaseZ = 3 - MathF.Pow(decreaseZ, 0.7f) * 3;
		for (int i = -5; i < 5; i++)
		{
			List<Vertex2D> bars = new List<Vertex2D>();
			for (int j = 0; j < 25; j++)
			{
				float yCoord = j / 30f + (float)Main.time * 0.007f;
				float zCoord0 = (j - 16) / 7f - MathF.Pow(Math.Abs(i), 2) / 25f - decreaseZ;
				float zCoord1 = (j - 16) / 7f - MathF.Pow(Math.Abs(i + 1), 2) / 25f - decreaseZ;
				if (j == 24)
				{
					zCoord0 = 0.3f;
					zCoord1 = 0.3f;
				}
				float widthCoord = 0.05f;
				float posY = (j * j - 576) * Projectile.scale;
				AddVertex(bars, Projectile.Center + new Vector2(i * 8 * (25 - j) * 0.2f, posY), new Vector3(0 + i * widthCoord, yCoord, zCoord0));
				AddVertex(bars, Projectile.Center + new Vector2((i + 1) * 8 * (25 - j) * 0.2f, posY), new Vector3(widthCoord + i * widthCoord, yCoord, zCoord1));
			}
			Main.graphics.graphicsDevice.Textures[0] = ModAsset.BloodFountain_Heatmap.Value;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 pos, Vector3 coords)
	{
		bars.Add(pos, Lighting.GetColor(pos.ToTileCoordinates()), coords);
	}
}
