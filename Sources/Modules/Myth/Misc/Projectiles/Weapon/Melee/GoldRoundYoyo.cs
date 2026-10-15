using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons.Yoyos;
using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

public class GoldRoundYoyo : YoyoProjectile
{
	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 380;
		ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 120;
		ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 20;
	}

	public override void SetCustomDefaults()
	{
		RotationalSpeed = 0.3f;
	}

	private float timer;
	private float trailWidth;
	private int hitCounter;
	public float Power;

	public override void OnSpawn(IEntitySource source)
	{
		trailWidth = 18f;
		hitCounter = 0;
		Power = 0;
	}

	public override void PostAI()
	{
		Player player = Main.player[Projectile.owner];
		if (Power > 400)
		{
			Power *= 0.999f;

			foreach (NPC npc in Main.npc)
			{
				if (Main.rand.NextBool(60)
					&& npc != null && npc.active && !npc.dontTakeDamage && !npc.friendly
					&& player.ownedProjectileCounts[ModContent.ProjectileType<FocusRay>()] < 5
					&& (Projectile.Center - npc.Center).Length() < 120)
				{
					var checkNPCList = new List<int>();
					foreach (Projectile proj in Main.projectile)
					{
						if (proj != null && proj.active)
						{
							if (proj.owner == Projectile.owner)
							{
								if (proj.type == ModContent.ProjectileType<FocusRay>())
								{
									FocusRay fcR0 = proj.ModProjectile as FocusRay;
									if (fcR0 != null)
									{
										checkNPCList.Add(fcR0.StickTarget.whoAmI);
									}
								}
							}
						}
					}
					if (!checkNPCList.Contains(npc.whoAmI))
					{
						Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), npc.Center, Vector2.zeroVector, ModContent.ProjectileType<FocusRay>(), Projectile.damage * 2, 0, Projectile.owner);
						FocusRay fcR = p0.ModProjectile as FocusRay;
						if (fcR != null)
						{
							fcR.StickTarget = npc;
							fcR.Yoyo = Projectile;
						}
					}
				}
			}
		}
		timer++;
		if (Projectile.ai[0] < 0)
		{
			trailWidth *= 0.8f;
		}
		Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0);
		Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, DustID.GemTopaz, 0f, 0f, 100, default, 0.7f);
		d.velocity = new Vector2(0, 2).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
		d.position += d.velocity * 2;
		d.noGravity = true;

		var spark = new RayDustDust
		{
			Velocity = new Vector2(0, Main.rand.NextFloat(0, 4f)).RotateRandom(MathHelper.TwoPi),
			Active = true,
			Visible = true,
			Position = Projectile.Center,
			MaxTime = Main.rand.Next(57, 155),
			Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(1f, 7.0f)),
			Rotation = Main.rand.NextFloat(6.283f),
			ai = new float[] { 0, 0 },
		};
		Ins.VFXManager.Add(spark);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		hitCounter++;
		Power++;
		if (hitCounter >= 6)
		{
			hitCounter = 0;
			for (int x = 0; x < 15; x++)
			{
				Vector2 v0 = new Vector2(Main.rand.NextFloat(3f, 12f), 0).RotatedByRandom(MathHelper.TwoPi);
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, v0, ModContent.ProjectileType<GoldRound>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
			}
			for (int x = 0; x < 75; x++)
			{
				var spark = new RayDustDust
				{
					Velocity = new Vector2(0, Main.rand.NextFloat(0, 16f)).RotateRandom(MathHelper.TwoPi),
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = Main.rand.Next(57, 255),
					Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(4f, 17.0f)),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0 },
				};
				Ins.VFXManager.Add(spark);
			}
		}
		for (int i = 0; i < 15; i++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(1.5f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			Dust d = Dust.NewDustDirect(Projectile.Center - new Vector2(4), 0, 0, DustID.GemTopaz, 0f, 0f, 100, default, 1.2f);
			d.velocity *= v;
			d.noGravity = true;
		}
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void PostDraw(Color lightColor)
	{
		Texture2D texture = ModAsset.GoldRoundYoyoGlow.Value;
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(1f, 0.7f, 0.1f, 0), Projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), Projectile.scale, SpriteEffects.None, 0);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawCorona();
		DrawYoyo_String();
		DrawCorona2();
		DrawBurningLine();
		Texture2D texture = ModAsset.Melee_GoldRoundYoyo.Value;
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public void DrawCorona()
	{
		Color drawC = new Color(1f, 0.4f, 0.05f, 0) * (0.5f * trailWidth / 18f);
		float timeValue = (float)Main.time * 0.0015f + MathF.Sin(Projectile.whoAmI);
		var bars = new List<Vertex2D>();

		for (int i = 0; i <= 120; ++i)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			float factor = i / 120f;
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 8).RotatedBy(MathHelper.TwoPi * factor), drawC, new Vector3(factor, timeValue, 0)));
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 35).RotatedBy(MathHelper.TwoPi * factor), Color.Transparent, new Vector3(factor, timeValue, 0)));
		}
		for (int i = 0; i <= 120; ++i)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			float factor = i / 120f;
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 8).RotatedBy(MathHelper.TwoPi * factor), drawC, new Vector3(factor, timeValue, 0)));
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 25).RotatedBy(MathHelper.TwoPi * factor), Color.Transparent, new Vector3(factor, timeValue, 0)));
		}
		for (int i = 0; i <= 120; ++i)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			float factor = i / 120f;
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 8).RotatedBy(MathHelper.TwoPi * factor), drawC, new Vector3(factor, timeValue, 0)));
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 15).RotatedBy(MathHelper.TwoPi * factor), Color.Transparent, new Vector3(factor, timeValue, 0)));
		}
		for (int i = 0; i <= 120; ++i)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			float factor = i / 120f;
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 8).RotatedBy(MathHelper.TwoPi * factor), drawC, new Vector3(factor, timeValue, 0)));
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 10).RotatedBy(MathHelper.TwoPi * factor), Color.Transparent, new Vector3(factor, timeValue, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_spiderNet.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public void DrawCorona2()
	{
		float mulColor = (Power - 300f) / 600f;
		if (mulColor > 1)
		{
			mulColor = 1;
		}
		Color drawC = new Color(1f, 0.4f, 0.05f, 0) * (0.5f * trailWidth / 18f) * mulColor;
		float timeValue = (float)Main.time * 0.00015f + MathF.Sin(Projectile.whoAmI);
		var bars = new List<Vertex2D>();

		for (int i = 0; i <= 120; ++i)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			float factor = i / 120f;
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 120).RotatedBy(MathHelper.TwoPi * factor), drawC, new Vector3(factor, timeValue, 0)));
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 240 * (MathF.Sin(i / 6f * MathF.PI + timeValue * 250) * 0.3f + 1)).RotatedBy(MathHelper.TwoPi * factor), Color.Transparent, new Vector3(factor, timeValue, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_hiveNet.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		bars = new List<Vertex2D>();

		for (int i = 0; i <= 120; ++i)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			float factor = i / 120f;
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 120).RotatedBy(MathHelper.TwoPi * factor), drawC, new Vector3(factor, timeValue + 0.4f, 0)));
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 180 * (MathF.Sin(i / 6f * MathF.PI + timeValue * 250 + MathF.PI) * 0.3f + 1)).RotatedBy(MathHelper.TwoPi * factor), Color.Transparent, new Vector3(factor, timeValue + 0.4f, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_hiveNet.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		bars = new List<Vertex2D>();
		for (int i = 0; i <= 120; ++i)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			float factor = i / 120f + timeValue * 360;
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 120).RotatedBy(MathHelper.TwoPi * factor), drawC, new Vector3(factor, 0.5f, 0)));
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 150).RotatedBy(MathHelper.TwoPi * factor), Color.Transparent, new Vector3(factor, 1f, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_6.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		bars = new List<Vertex2D>();
		for (int i = 0; i <= 120; ++i)
		{
			Vector2 drawPos = Projectile.Center - Main.screenPosition;
			float factor = i / 120f + timeValue * 360;
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 150).RotatedBy(MathHelper.TwoPi * factor), drawC, new Vector3(factor, 0f, 0)));
			bars.Add(new Vertex2D(drawPos + new Vector2(0, 210).RotatedBy(MathHelper.TwoPi * factor), Color.Transparent, new Vector3(factor, 1f, 0)));
		}
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Noise_SolarSpectrum.Value;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public void DrawBurningLine()
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
		Effect effect = Commons.ModAsset.Trailing.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.CurrentTechnique.Passes[0].Apply();
		foreach (Projectile proj in Main.projectile)
		{
			if (proj != null && proj.active)
			{
				if (proj.owner == Projectile.owner)
				{
					if (proj.type == ModContent.ProjectileType<FocusRay>())
					{
						float distanceProj = (Projectile.Center - proj.Center).Length();
						if (distanceProj <= 120)
						{
							Color drawC = new Color(1f, 0.4f, 0.05f, 0) * (0.5f * trailWidth / 18f);
							float timeValue = (float)Main.time * 0.05f + MathF.Sin(Projectile.whoAmI);
							Vector2 normalized = Vector2.Normalize(Projectile.Center - proj.Center);
							Vector2 drawPoint = proj.Center;
							Vector2 normalizedLeft = normalized.RotatedBy(MathHelper.PiOver2);
							float drawWidth = Math.Clamp(proj.timeLeft / 30f, 0, 1);
							drawWidth *= 15f;
							var bars = new List<Vertex2D>();
							float oldFactor = 0;
							for (int i = 1; i <= 200; ++i)
							{
								drawPoint += normalized * 3;
								float factor = i / 20f;
								float mulWidth = 1;
								float distance = (drawPoint - Projectile.Center).Length();
								if (distance < 8f)
								{
									mulWidth = distance / 8f;
									mulWidth = MathF.Sqrt(mulWidth);
								}
								bars.Add(new Vertex2D(drawPoint + normalizedLeft * drawWidth, drawC, new Vector3(factor + timeValue, 0, mulWidth)));
								bars.Add(new Vertex2D(drawPoint - normalizedLeft * drawWidth, drawC, new Vector3(factor + timeValue, 1, mulWidth)));
								if (distance < 4)
								{
									break;
								}
								oldFactor = factor;
							}
							bars.Add(new Vertex2D(Projectile.Center + normalizedLeft * drawWidth, drawC, new Vector3(oldFactor + timeValue, 0, 0)));
							bars.Add(new Vertex2D(Projectile.Center - normalizedLeft * drawWidth, drawC, new Vector3(oldFactor + timeValue, 1, 0)));
							Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
							Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
							if (bars.Count > 3)
							{
								Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
							}
						}
						else
						{
							if (proj.timeLeft > 1)
							{
								proj.timeLeft = 1;
							}
						}
					}
				}
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public float TrailWidthFunction(float factor)
	{
		factor *= 6;
		factor -= 1;
		if (factor < 0)
		{
			return MathF.Pow(MathF.Cos(factor * MathHelper.PiOver2), 0.5f);
		}
		return MathF.Pow(MathF.Cos(factor / 5f * MathHelper.PiOver2), 3);
	}
}
