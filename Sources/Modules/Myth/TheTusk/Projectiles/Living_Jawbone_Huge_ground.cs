using Everglow.Commons.DataStructures;
using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.TheTusk.Projectiles;

// ai0和ai1用来存左,右两侧颌骨的张开度
public class Living_Jawbone_Huge_ground : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public List<Vector2> RotationBaseUp = new List<Vector2>();
	public List<Vector2> RotationBaseDown = new List<Vector2>();
	public Vector2 DiveOffset = default;

	public override string Texture => ModAsset.Living_Jawbone_Huge_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 100;
		Projectile.height = 100;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 180;
		Projectile.alpha = 255;
		Projectile.penetrate = -1;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Vector2 checkDir = TileUtils.GetTopographicGradient(Projectile.Center, 20);
		for (int y = 0; y < 100; y++)
		{
			if (Collision.SolidCollision(Projectile.Center - new Vector2(8), 16, 16))
			{
				break;
			}
			Projectile.position -= checkDir * 10;
			if (y == 99)
			{
				Projectile.active = false;
			}
		}
		Projectile.rotation = TileUtils.GetTopographicGradient(Projectile.Center, 20).ToRotation();
		Projectile.position -= Projectile.rotation.ToRotationVector2() * 20;
		for (int t = 0; t < 100; t++)
		{
			Vector2 check_up = Projectile.Center + new Vector2(450, 0).RotatedBy(Projectile.rotation - Projectile.ai[0]);
			if (GetToCenterCollision20TimesIn450Range(check_up) > 18)
			{
				break;
			}
			else
			{
				Projectile.ai[0] += 0.05f;
			}
			if (t == 99)
			{
				Projectile.active = false;
			}
		}
		for (int t = 0; t < 100; t++)
		{
			Vector2 check_down = Projectile.Center + new Vector2(450, 0).RotatedBy(Projectile.rotation + Projectile.ai[1]);
			if (GetToCenterCollision20TimesIn450Range(check_down) > 18)
			{
				break;
			}
			else
			{
				Projectile.ai[1] += 0.05f;
			}
			if (t == 99)
			{
				Projectile.active = false;
			}
		}
		for (int i = 0; i < 20; i++)
		{
			RotationBaseUp.Add(new Vector2(18 * i, 0).RotatedBy(Projectile.rotation - Projectile.ai[0]) + Projectile.Center);
			RotationBaseDown.Add(new Vector2(18 * i, 0).RotatedBy(Projectile.rotation + Projectile.ai[1]) + Projectile.Center);
		}
	}

	/// <summary>
	/// 查找沿Proj中心点到此点方向上640距离的20次碰撞次数
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	public int GetToCenterCollision20TimesIn450Range(Vector2 position)
	{
		int times = 0;
		if (position == Projectile.Center)
		{
			return times;
		}
		Vector2 normal = Vector2.Normalize(position - Projectile.Center);
		for (int i = 0; i < 20; i++)
		{
			Vector2 checkPoint = normal * i * 32 + Projectile.Center;
			if (Collision.SolidCollision(checkPoint, 0, 0))
			{
				times++;
			}
		}
		return times;
	}

	public void UpdateRotationBase()
	{
		List<Vector2> newRotationBaseUp = new List<Vector2>();
		for (int i = 0; i < 20; i++)
		{
			if (Collision.SolidCollision(RotationBaseUp[i], 0, 0))
			{
				Vector2 boneUp = new Vector2(360, 0).RotatedBy(Projectile.rotation - Projectile.ai[0]) * i / 20f + Projectile.Center;
				if (Collision.SolidCollision(boneUp, 0, 0))
				{
					newRotationBaseUp.Add(boneUp);
				}
				else
				{
					newRotationBaseUp.Add(RotationBaseUp[i]);
				}
			}
			else
			{
				newRotationBaseUp.Add(RotationBaseUp[i]);
			}
		}
		RotationBaseUp = newRotationBaseUp;

		List<Vector2> newRotationBaseDown = new List<Vector2>();
		for (int i = 0; i < 20; i++)
		{
			if (Collision.SolidCollision(RotationBaseDown[i], 0, 0))
			{
				Vector2 boneDown = new Vector2(360, 0).RotatedBy(Projectile.rotation + Projectile.ai[1]) * i / 20f + Projectile.Center;
				if (Collision.SolidCollision(boneDown, 0, 0))
				{
					newRotationBaseDown.Add(boneDown);
				}
				else
				{
					newRotationBaseDown.Add(RotationBaseDown[i]);
				}
			}
			else
			{
				newRotationBaseDown.Add(RotationBaseDown[i]);
			}
		}
		RotationBaseDown = newRotationBaseDown;
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		if (Projectile.alpha > 0)
		{
			Projectile.alpha -= 8;
			Projectile.timeLeft = 180;
		}
		else
		{
			Projectile.alpha = 0;
		}
		if (Projectile.timeLeft is < 180 and > 160)
		{
			if (Projectile.ai[0] > 1.4f)
			{
				Projectile.ai[0] = (float)Utils.Lerp(Projectile.ai[0], 1.4f, 0.3f);
			}
			if (Projectile.ai[1] > 1.4f)
			{
				Projectile.ai[1] = (float)Utils.Lerp(Projectile.ai[1], 1.4f, 0.3f);
			}

			UpdateRotationBase();
		}
		if (Projectile.timeLeft is < 160 and > 130)
		{
			if (Projectile.ai[0] > 1.4f)
			{
				Projectile.ai[0] += 0.01f;
			}
			if (Projectile.ai[1] > 1.4f)
			{
				Projectile.ai[1] += 0.01f;
			}
		}
		if (Projectile.timeLeft == 170)
		{
			SoundEngine.PlaySound(new SoundStyle(ModAsset.TuskJawBoneBite_Mod).WithVolumeScale(Main.rand.NextFloat(0.8f, 1f)), Projectile.Center);
		}
		if (Projectile.timeLeft == 121)
		{
			for (int i = 0; i < 20; i++)
			{
				Vector2 pos0 = new Vector2(MathF.Sqrt(Main.rand.NextFloat(1f)) * 430 * Projectile.scale, 0).RotatedBy(Projectile.rotation - Projectile.ai[0] / 20f * i);
				var blood = new BloodDrop
				{
					Velocity = pos0.RotatedBy(MathHelper.PiOver2) * 0.1f,
					Active = true,
					Visible = true,
					Position = pos0 + Projectile.Center,
					MaxTime = Main.rand.Next(54, 74),
					Scale = Main.rand.NextFloat(6f, 25f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
				};
				Ins.VFXManager.Add(blood);
				var blood2 = new BloodSplash
				{
					Velocity = pos0.RotatedBy(MathHelper.PiOver2) * 0.1f,
					Active = true,
					Visible = true,
					Position = pos0 + Projectile.Center,
					MaxTime = Main.rand.Next(54, 74),
					Scale = Main.rand.NextFloat(6f, 18f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
				};
				Ins.VFXManager.Add(blood2);
			}
			for (int i = 0; i < 20; i++)
			{
				Vector2 pos1 = new Vector2(MathF.Sqrt(Main.rand.NextFloat(1f)) * 430 * Projectile.scale, 0).RotatedBy(Projectile.rotation + Projectile.ai[1] / 20f * i);
				var blood = new BloodDrop
				{
					Velocity = pos1.RotatedBy(-MathHelper.PiOver2) * 0.1f,
					Active = true,
					Visible = true,
					Position = pos1 + Projectile.Center,
					MaxTime = Main.rand.Next(54, 74),
					Scale = Main.rand.NextFloat(6f, 25f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
				};
				Ins.VFXManager.Add(blood);
				var blood2 = new BloodSplash
				{
					Velocity = pos1.RotatedBy(-MathHelper.PiOver2) * 0.1f,
					Active = true,
					Visible = true,
					Position = pos1 + Projectile.Center,
					MaxTime = Main.rand.Next(54, 74),
					Scale = Main.rand.NextFloat(6f, 18f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
				};
				Ins.VFXManager.Add(blood2);
			}
		}
		if (Projectile.timeLeft == 120)
		{
			Projectile.ai[0] = 0;
			Projectile.ai[1] = 0;
			ShakerManager.AddShaker(Projectile.Center, Projectile.rotation.ToRotationVector2(), 200, 60, 120);
			Collision.HitTiles(Projectile.position - new Vector2(300), Projectile.rotation.ToRotationVector2() * 3, Projectile.width + 600, Projectile.height + 600);
			Vector2 vel = Projectile.rotation.ToRotationVector2() * 45f;
			foreach (Dust dust in Main.dust)
			{
				if (dust.active)
				{
					float distance = (dust.position - Projectile.Center).Length();
					if (distance < 100)
					{
						dust.velocity += vel * (100 - distance) / 100f;
					}
				}
			}
		}
		if (Projectile.timeLeft == 115)
		{
			Projectile.damage /= 5;
		}
		if (Projectile.timeLeft is < 90 and > 60)
		{
			Projectile.ai[0] = (float)Utils.Lerp(Projectile.ai[0], 0.4f, 0.05f);
			Projectile.ai[1] = (float)Utils.Lerp(Projectile.ai[0], 0.4f, 0.05f);
		}
		if (Projectile.timeLeft < 60)
		{
			Projectile.ai[0] = (float)Utils.Lerp(Projectile.ai[0], 0.0f, 0.05f);
			Projectile.ai[1] = (float)Utils.Lerp(Projectile.ai[0], 0.0f, 0.05f);

			float depth = (60 - Projectile.timeLeft) / 60f;
			depth *= depth * 450;
			DiveOffset = new Vector2(-depth, 0).RotatedBy(Projectile.rotation);
			for (int i = 0; i < 6; i++)
			{
				Vector2 position = Projectile.Center + new Vector2(Main.rand.NextFloat(-80, 80), -30).RotatedBy(Projectile.rotation + MathHelper.PiOver2);
				if (Collision.SolidCollision(position, 0, 0))
				{
					for (int j = 0; j < 20; j++)
					{
						position += new Vector2(4, 0).RotatedBy(Projectile.rotation);
						if (!Collision.SolidCollision(position, 0, 0))
						{
							break;
						}
					}
				}
				else
				{
					for (int j = 0; j < 20; j++)
					{
						position -= new Vector2(4, 0).RotatedBy(Projectile.rotation);
						if (Collision.SolidCollision(position, 0, 0))
						{
							break;
						}
					}
				}
				Dust dust = Dust.NewDustDirect(position - new Vector2(4), 0, 0, DustID.Blood);
				dust.scale = Main.rand.NextFloat(0.9f, 2.6f);
				dust.noGravity = true;
				dust.velocity *= 0;
			}
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float k = 0;
		float hitRange = MathF.Max(430 - DiveOffset.Length(), 0);
		if (Projectile.timeLeft == 121)
		{
			for (int i = 0; i < 20; i++)
			{
				bool b2 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(hitRange * Projectile.scale, 0).RotatedBy(Projectile.rotation - Projectile.ai[0] / 20f * i), 60, ref k);
				bool b3 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(hitRange * Projectile.scale, 0).RotatedBy(Projectile.rotation + Projectile.ai[1] / 20f * i), 60, ref k);
				if (b2 || b3)
				{
					return true;
				}
			}
		}
		bool b0 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(hitRange * Projectile.scale, 0).RotatedBy(Projectile.rotation - Projectile.ai[0]), 60, ref k);
		bool b1 = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(hitRange * Projectile.scale, 0).RotatedBy(Projectile.rotation + Projectile.ai[1]), 60, ref k);
		return b0 || b1;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		Projectile.hide = true;
		behindNPCsAndTiles.Add(index);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		BeginDissolveShader(Math.Clamp(Projectile.timeLeft / 50f - 0.2f, -0.2f, 1), new Vector4(0.3f, 0f, 0f, 0.7f));
		Texture stickyBlood = ModAsset.Living_Jawbone_BloodLines.Value;

		List<Vertex2D> bars = new List<Vertex2D>();
		Vector2 bonePlaceUp = new Vector2(360, 0).RotatedBy(Projectile.rotation - Projectile.ai[0] - 0.14f);
		for (int i = 0; i < 20; i++)
		{
			AddVertex(bars, RotationBaseUp[i], new Vector3(i / 10f, 1, 0));
			AddVertex(bars, bonePlaceUp * i / 20f + Projectile.Center + DiveOffset, new Vector3(i / 10f, 0, 0));
		}
		Main.graphics.graphicsDevice.Textures[0] = stickyBlood;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		bars = new List<Vertex2D>();
		Vector2 bonePlaceDown = new Vector2(360, 0).RotatedBy(Projectile.rotation + Projectile.ai[1] + 0.14f);
		for (int i = 0; i < 20; i++)
		{
			AddVertex(bars, RotationBaseDown[i], new Vector3(i / 10f, 1, 0));
			AddVertex(bars, bonePlaceDown * i / 20f + Projectile.Center + DiveOffset, new Vector3(i / 10f, 0, 0));
		}
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		BeginDissolveShader(1f, new Vector4(0.5f, 0f, 0f, 0.7f));
		Texture2D upJaw = ModAsset.Living_Jawbone_Huge_up.Value;
		bonePlaceUp = new Vector2(upJaw.Width, 0).RotatedBy(Projectile.rotation - Projectile.ai[0]);

		bars = new List<Vertex2D>();
		bool insertTile = false;
		for (int i = 10; i >= 0; i--)
		{
			Vector2 width = Vector2.Normalize(bonePlaceUp.RotatedBy(MathHelper.PiOver2)) * upJaw.Height * 0.5f;
			AddVertex(bars, bonePlaceUp * i / 10f + width + Projectile.Center + DiveOffset, new Vector3(i / 10f, 1, 0), insertTile);
			AddVertex(bars, bonePlaceUp * i / 10f - width + Projectile.Center + DiveOffset, new Vector3(i / 10f, 0, 0), insertTile);
			if (!insertTile && Collision.SolidCollision(bonePlaceUp * i / 10f + Projectile.Center + DiveOffset, 0, 0) && Projectile.timeLeft < 60)
			{
				insertTile = true;
			}
		}
		Main.graphics.graphicsDevice.Textures[0] = upJaw;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		Texture2D downJaw = ModAsset.Living_Jawbone_Huge_down.Value;
		bonePlaceDown = new Vector2(downJaw.Width, 0).RotatedBy(Projectile.rotation + Projectile.ai[1]);
		bars = new List<Vertex2D>();
		insertTile = false;
		for (int i = 10; i >= 0; i--)
		{
			Vector2 width = Vector2.Normalize(bonePlaceDown.RotatedBy(MathHelper.PiOver2)) * downJaw.Height * 0.5f;
			AddVertex(bars, bonePlaceDown * i / 10f + width + Projectile.Center + DiveOffset, new Vector3(i / 10f, 1, 0), insertTile);
			AddVertex(bars, bonePlaceDown * i / 10f - width + Projectile.Center + DiveOffset, new Vector3(i / 10f, 0, 0), insertTile);
			if (!insertTile && Collision.SolidCollision(bonePlaceUp * i / 10f + Projectile.Center + DiveOffset, 0, 0) && Projectile.timeLeft < 60)
			{
				insertTile = true;
			}
		}
		Main.graphics.graphicsDevice.Textures[0] = downJaw;
		Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 pos, Vector3 coords, bool transparent = false)
	{
		bars.Add(pos, Lighting.GetColor(pos.ToTileCoordinates()) * (transparent ? 0 : 1), coords);
	}

	public void BeginDissolveShader(float dissolveValue, Vector4 dissolveColor)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null);
		Effect dissolve = ModAsset.Living_Jawbone_Huge_dissolve.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		float dissolveDuration = dissolveValue;
		dissolve.Parameters["uTransform"].SetValue(model * projection);
		dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
		dissolve.Parameters["duration"].SetValue(dissolveDuration);
		dissolve.Parameters["uDissolveColor"].SetValue(dissolveColor);
		dissolve.Parameters["uNoiseSize"].SetValue(1f);
		dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(Projectile.ai[1], Projectile.ai[2]));
		dissolve.CurrentTechnique.Passes[0].Apply();
	}
}
