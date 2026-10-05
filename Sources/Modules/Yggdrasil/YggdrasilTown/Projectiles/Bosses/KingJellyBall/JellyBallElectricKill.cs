using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.KingJellyBall;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.KingJellyBall;

public class JellyBallElectricKill : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public NPC OwnerBoss;
	public NPC Killee;
	public List<Vector2> FlowPosList = new List<Vector2>();

	public override void SetDefaults()
	{
		Projectile.timeLeft = 120;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = -1;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public override void AI()
	{
		if (OwnerBoss == null || !OwnerBoss.active || OwnerBoss.type != ModContent.NPCType<NPCs.KingJellyBall.KingJellyBall>() || OwnerBoss.life <= 0)
		{
			if (Projectile.timeLeft > 60)
			{
				Projectile.timeLeft = 60;
				Projectile.hostile = false;
			}
			return;
		}
		if (Killee == null || !Killee.active || Killee.type != ModContent.NPCType<JellyBall>() && Killee.type != ModContent.NPCType<GiantJellyBall>() || Killee.life <= 0)
		{
			if (Projectile.timeLeft > 60)
			{
				Projectile.timeLeft = 60;
				Projectile.hostile = false;
			}
			return;
		}
		if (Projectile.timeLeft == 24)
		{
			float power = 5f;
			if (Killee.type == ModContent.NPCType<GiantJellyBall>())
			{
				power = 10f;
			}
			int damage = (int)(Projectile.damage / 1.1f * 1.8f);
			if (power >= 10f)
			{
				damage = (int)(Projectile.damage / 1.1f * 2.5f);
			}
			Killee.StrikeNPC(Main.rand.Next(350, 450), 0, 0);
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Killee.Center, Vector2.zeroVector, ModContent.ProjectileType<JellyBall_Electric_Explosion>(), damage, 5, Projectile.owner, power);
		}
		Projectile.Center = OwnerBoss.Center;
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Projectile.timeLeft > 30)
		{
			return false;
		}
		foreach (var vec in FlowPosList)
		{
			if (targetHitbox.Contains(vec.ToPoint()))
			{
				return true;
			}
		}
		return false;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		if (OwnerBoss == null || !OwnerBoss.active || OwnerBoss.type != ModContent.NPCType<NPCs.KingJellyBall.KingJellyBall>() || OwnerBoss.life <= 0)
		{
			if (Projectile.timeLeft > 60)
			{
				Projectile.timeLeft = 60;
			}
			return false;
		}
		float timeValue = (float)Main.time * 0.03f;
		float mulColor = 1f;
		float width = 30f;
		if (Killee.type == ModContent.NPCType<GiantJellyBall>())
		{
			width = 50f;
		}
		if (Projectile.timeLeft > 30)
		{
			width *= Projectile.timeLeft / 120f;
		}
		else
		{
			width *= 0.5f + MathF.Sin(Projectile.timeLeft * 0.3f);
			width *= 0.15f;
		}
		Vector2 bossCenter = OwnerBoss.Center + new Vector2(0, -120 * (OwnerBoss.scale - 0.3f));
		var flowPosList = new List<Vector2>();
		Vector2 flowPos = Killee.Center;
		Vector2 flowVelocity = Vector2.Normalize(bossCenter - Killee.Center).RotatedBy(MathF.Sin(timeValue + Killee.whoAmI) * 0.5f) * 6;
		if (Projectile.hostile)
		{
			if (Projectile.timeLeft <= 30)
			{
				flowVelocity = flowVelocity.RotatedBy(MathF.Sin(timeValue * 0.7f + Killee.whoAmI) * 0.2f);
			}
		}
		for (int i = 0; i < 600; i++)
		{
			flowPosList.Add(flowPos);
			flowPos += flowVelocity;
			Vector2 toTarget = bossCenter - flowPos - flowVelocity;
			if (toTarget.Length() >= 20)
			{
				flowVelocity = Vector2.Normalize(toTarget) * 0.05f * 6f + flowVelocity * 0.95f;
				if (Projectile.hostile)
				{
					if (Projectile.timeLeft <= 30 && i % 7 == 3)
					{
						flowVelocity = flowVelocity.RotatedBy(MathF.Sin(timeValue * 2.2f + Killee.whoAmI + i) * 0.4f);
						flowVelocity = flowVelocity.RotatedBy(MathF.Sin(timeValue * 1.07f + Killee.whoAmI + i * 0.5f) * 0.3f);
						flowVelocity = flowVelocity.RotatedBy(MathF.Sin(timeValue * 0.48f + Killee.whoAmI + i * 0.15f) * 0.2f);
						Lighting.AddLight(flowPos, new Vector3(0.7f, 1f, 1f) * width / 4f);
					}
					if (!Main.gamePaused)
					{
						if (Projectile.timeLeft == 1)
						{
							var dustVFX = new JellyBallSparkElectricity
							{
								Velocity = flowVelocity * 0.5f,
								Active = true,
								Visible = true,
								Position = flowPos,
								MaxTime = Main.rand.Next(30, 50),
								Scale = Main.rand.Next(1, 2) * Projectile.ai[0],
								ai = new float[] { Main.rand.NextFloat(1f, 8f), 0 },
							};
							Ins.VFXManager.Add(dustVFX);
						}
					}
				}
			}
			else
			{
				break;
			}
		}
		FlowPosList = flowPosList;
		Color drawColor = new Color(0.05f, 0.03f, 1f, 0f) * mulColor;
		Color drawColor2 = new Color(0.95f, 1f, 1f, 0f) * mulColor;
		drawColor = Color.Lerp(drawColor, drawColor2, 1 - MathF.Pow(Projectile.timeLeft / 120f, 1f / 3));
		Color drawColor_dark = new Color(1f, 1f, 1f, 1f) * mulColor * 0.3f;

		var powerFlow = new List<Vertex2D>();
		var powerFlow_dark = new List<Vertex2D>();
		if (flowPosList.Count > 2)
		{
			for (int i = 0; i < flowPosList.Count - 1; i++)
			{
				float mulColor2 = 1f;
				if (i < 3)
				{
					mulColor2 = i / 3f;
				}
				if (i >= flowPosList.Count - 4)
				{
					mulColor2 = (flowPosList.Count - 2 - i) / 3f;
				}
				Vector2 normal = (flowPosList[i] - flowPosList[i + 1]).SafeNormalize(Vector2.One);
				normal = normal.RotatedBy(MathHelper.PiOver2) * width;
				powerFlow.Add(flowPosList[i] + normal - Main.screenPosition, drawColor * mulColor2, new Vector3(i * 0.05f - timeValue, 0, 0));
				powerFlow.Add(flowPosList[i] - normal - Main.screenPosition, drawColor * mulColor2, new Vector3(i * 0.05f - timeValue, 1, 0));
				powerFlow_dark.Add(flowPosList[i] + normal - Main.screenPosition, drawColor_dark * mulColor2, new Vector3(i * 0.05f - timeValue, 0, 0));
				powerFlow_dark.Add(flowPosList[i] - normal - Main.screenPosition, drawColor_dark * mulColor2, new Vector3(i * 0.05f - timeValue, 1, 0));
			}
		}

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		float lightValue = 0.5f + MathF.Sin(Projectile.timeLeft * 0.4f);
		lightValue *= 2;
		if (powerFlow.Count > 3)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_10_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, powerFlow_dark.ToArray(), 0, powerFlow_dark.Count - 2);
			if (Projectile.timeLeft > 30)
			{
				if (Projectile.hostile)
				{
					Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_10.Value;
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, powerFlow.ToArray(), 0, powerFlow.Count - 2);
				}
			}
			else
			{
				for (int i = 0; i < lightValue; i++)
				{
					Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_7.Value;
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, powerFlow.ToArray(), 0, powerFlow.Count - 2);
				}
			}
		}
		if (Projectile.timeLeft is > 30 and < 150)
		{
			float mulColor2 = 1f;
			if (Projectile.timeLeft > 120)
			{
				mulColor2 = (150 - Projectile.timeLeft) / 30f;
			}

			var powerCircleLarge = new List<Vertex2D>();
			for (int i = 0; i <= 50; i++)
			{
				Vector2 radius = new Vector2(Projectile.timeLeft + 60).RotatedBy(i / 50f * MathHelper.TwoPi) * 0.5f;
				Vector2 radius2 = new Vector2(Projectile.timeLeft - 30).RotatedBy(i / 50f * MathHelper.TwoPi) * 0.5f;
				powerCircleLarge.Add(Killee.Center + radius - Main.screenPosition, drawColor * mulColor2 * 0.5f, new Vector3(i / 25f - timeValue * 0.25f + Killee.whoAmI, 0, 0));
				powerCircleLarge.Add(Killee.Center + radius2 - Main.screenPosition, drawColor * mulColor2 * 0.7f, new Vector3(i / 25f - timeValue * 0.25f + Killee.whoAmI, 1, 0));
			}
			if (powerCircleLarge.Count > 3)
			{
				Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_thick.Value;
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, powerCircleLarge.ToArray(), 0, powerCircleLarge.Count - 2);
			}
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		return false;
	}
}
