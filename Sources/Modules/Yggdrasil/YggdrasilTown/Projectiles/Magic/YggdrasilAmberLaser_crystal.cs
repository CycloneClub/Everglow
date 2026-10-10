using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class YggdrasilAmberLaser_crystal : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.magic = true;
		Projectile.friendly = true;
		Projectile.timeLeft = 900;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		CrystalTrack = new Vector2[50];
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
		Projectile.usesOwnerLight = true;
	}

	public Projectile LaserOwner;

	public Vector2[] CrystalTrack = new Vector2[50];

	public bool EndAI = false;

	public override void OnSpawn(IEntitySource source)
	{
		CrystalTrack[0] = Projectile.Center;
		Projectile.ai[0] = 0;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<AmberStick>(), 6);
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void AI()
	{
		if (Projectile.timeLeft < 60)
		{
			int length = 0;
			for (int i = 0; i < CrystalTrack.Length - 1; i++)
			{
				if (CrystalTrack[i + 1] != Vector2.zeroVector)
				{
					length++;
				}
			}
			for (int y = 0; y < Math.Max(1, length / 2); y++)
			{
				if (length > 1 && Main.rand.NextBool(8))
				{
					int index = Main.rand.Next(1, length);
					var d = Dust.NewDustDirect(CrystalTrack[index] - new Vector2(4), 0, 0, ModContent.DustType<YggdrasilAmber_crack>());
					d.velocity = new Vector2(0, Main.rand.NextFloat(0, 0.035f)).RotatedByRandom(MathHelper.TwoPi);
					d.position += new Vector2(0, Main.rand.NextFloat(0f, 20f)).RotatedByRandom(MathHelper.TwoPi);
					d.scale *= 2f;
				}
			}
		}
		++Projectile.ai[0];
		if (!LaserOwner.active)
		{
			EndAI = true;
		}
		if (EndAI)
		{
			return;
		}
		if (LaserOwner != null && LaserOwner.type == ModContent.ProjectileType<YggdrasilAmberLaser_proj>())
		{
			var yALp = LaserOwner.ModProjectile as YggdrasilAmberLaser_proj;
			if (yALp != null)
			{
				Vector2 endPoint = yALp.EndPoint;
				for (int i = 1; i < CrystalTrack.Length; i++)
				{
					if (CrystalTrack[i] == default)
					{
						if ((CrystalTrack[i - 1] - endPoint).Length() is > 10 and < 30)
						{
							CrystalTrack[i] = endPoint;
							Projectile.ai[1] = 0;
						}
						else if ((CrystalTrack[i - 1] - endPoint).Length() is >= 30 and < 120)
						{
							CrystalTrack[i] = (endPoint + CrystalTrack[i - 1]) * 0.5f;
							CrystalTrack[i + 1] = endPoint;
							Projectile.ai[1] = 0;
						}
						else if ((CrystalTrack[i - 1] - endPoint).Length() >= 120 && i > 4)
						{
							EndAI = true;
							yALp.Crystalized = false;
							Projectile.ai[1] = 0;
						}
						else if ((CrystalTrack[i - 1] - endPoint).Length() <= 10)
						{
							Projectile.ai[1]++;
							if (i < 8 && Projectile.ai[1] > 4)
							{
								float randomRot = Main.rand.NextFloat(MathHelper.TwoPi);
								for (int j = 0; j < 9; j++)
								{
									CrystalTrack[j] = endPoint + new Vector2(j - 4).RotatedBy(randomRot) * 5;
									EndAI = true;
								}
							}
						}
						break;
					}
				}
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Projectile.timeLeft < 60)
		{
			return false;
		}
		for (int i = 0; i < CrystalTrack.Length - 1; i++)
		{
			if (CrystalTrack[i + 1] == default)
			{
				return false;
			}
			float value = 0;
			if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), CrystalTrack.ToArray()[i], CrystalTrack.ToArray()[i + 1], 20, ref value))
			{
				return true;
			}
		}
		return false;
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
	}



	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var bars = new List<Vertex2D>();

		var barsReflect = new List<Vertex2D>();
		if (CrystalTrack.Length > 1)
		{
			int length = 0;
			for (int i = 0; i < CrystalTrack.Length - 1; i++)
			{
				if (CrystalTrack[i + 1] != Vector2.zeroVector)
				{
					length++;
				}
			}
			for (int i = 0; i < CrystalTrack.Length - 1; i++)
			{
				float width = 25 + 6 * MathF.Sin(i * 4);
				if (i < 4)
				{
					width *= i / 4f;
				}
				if (length - i - 1 < 4)
				{
					width *= (length - i - 1) / 4f;
				}
				if (CrystalTrack[i + 1] != Vector2.zeroVector)
				{
					Vector2 normal = (CrystalTrack.ToArray()[i + 1] - CrystalTrack.ToArray()[i]).SafeNormalize(Vector2.zeroVector).RotatedBy(MathHelper.PiOver2);
					AddVertex(bars, CrystalTrack.ToArray()[i] + normal * width, new Vector3(i * 0.3f, 1, 0));
					AddVertex(bars, CrystalTrack.ToArray()[i] - normal * width, new Vector3(i * 0.3f, 0f, 0));

					AddVertex2(barsReflect, CrystalTrack.ToArray()[i] + normal * width, new Vector3(i * 0.3f, 1, 0));
					AddVertex2(barsReflect, CrystalTrack.ToArray()[i] - normal * width, new Vector3(i * 0.3f, 0f, 0));
				}
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
		}
		return false;
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 position, Vector3 texCoord)
	{
		float duration = 1f;
		if (Projectile.timeLeft < 60f)
		{
			duration = Projectile.timeLeft / 60f;
		}
		float value = 0f;
		if (Projectile.ai[0] < 200)
		{
			value = (200 - Projectile.ai[0]) / 200f;
		}
		Color color = Color.Lerp(Lighting.GetColor(position.ToTileCoordinates()) * 0.6f, new Color(1f, 1f, 0.4f, 0.9f), value) * duration;
		bars.Add(position - Main.screenPosition, color, texCoord);
	}

	public void AddVertex2(List<Vertex2D> bars, Vector2 position, Vector3 texCoord)
	{
		bars.Add(position - Main.screenPosition, Lighting.GetColor(position.ToTileCoordinates()) * 1.5f, texCoord);
	}
}
