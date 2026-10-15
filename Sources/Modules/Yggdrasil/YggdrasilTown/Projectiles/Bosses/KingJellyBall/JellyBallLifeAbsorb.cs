using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.KingJellyBall;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.KingJellyBall;

public class JellyBallLifeAbsorb : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public NPC OwnerBoss;
	public NPC Absorbee;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 1440;
		if (Main.expertMode)
		{
			Projectile.timeLeft = 1230;
		}
		if (Main.masterMode)
		{
			Projectile.timeLeft = 1200;
		}
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = -1;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;
	}

	public override void AI()
	{
		if (OwnerBoss == null || !OwnerBoss.active || OwnerBoss.type != ModContent.NPCType<NPCs.KingJellyBall.KingJellyBall>() || OwnerBoss.life <= 0)
		{
			if (Projectile.timeLeft > 60)
			{
				Projectile.timeLeft = 60;
			}
			return;
		}
		if (Absorbee == null || !Absorbee.active || Absorbee.type != ModContent.NPCType<JellyBall>() && Absorbee.type != ModContent.NPCType<GiantJellyBall>() || Absorbee.life <= 0)
		{
			if (Projectile.timeLeft > 60)
			{
				Projectile.timeLeft = 60;
			}
			return;
		}
		if (Projectile.timeLeft == 30)
		{
			if (Absorbee.type == ModContent.NPCType<JellyBall>())
			{
				OwnerBoss.HealEffect(100);
				OwnerBoss.life += 100;
				if (OwnerBoss.life > OwnerBoss.lifeMax)
				{
					OwnerBoss.life = OwnerBoss.lifeMax;
				}
			}
			if (Absorbee.type == ModContent.NPCType<GiantJellyBall>())
			{
				OwnerBoss.HealEffect(240);
				OwnerBoss.life += 240;
				if (OwnerBoss.life > OwnerBoss.lifeMax)
				{
					OwnerBoss.life = OwnerBoss.lifeMax;
				}
			}
			if (Main.expertMode)
			{
				var largeJelly = NPC.NewNPCDirect(Projectile.GetSource_FromAI(), OwnerBoss.Center, ModContent.NPCType<GiantJellyBall>(), default, default, 127);
				largeJelly.velocity = new Vector2(16, 0).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi));
			}
			var kingJellyBall = OwnerBoss.ModNPC as NPCs.KingJellyBall.KingJellyBall;
			if (kingJellyBall != null)
			{
				kingJellyBall.HealLightValue = 1f;
			}
			var healEffect = new KingJellyBall_Heal
			{
				Active = true,
				Visible = true,
				MyKingJellyBallOwner = OwnerBoss,
				Position = OwnerBoss.Center,
				Rotation = OwnerBoss.rotation,
				Scale = OwnerBoss.scale,
				Timer = 0,
				MaxTime = 30,
				ai = new float[] { Main.rand.NextFloat(20000f), Main.rand.NextFloat(20000f) },
			};
			Ins.VFXManager.Add(healEffect);
			Absorbee.active = false;
		}
		if (Projectile.timeLeft > 30 && Projectile.timeLeft < 60)
		{
			Absorbee.scale *= 0.9f;
		}
		Projectile.Center = OwnerBoss.Center;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float timeValue = (float)Main.time * 0.03f;
		float mulColor = 1f;
		if (Projectile.timeLeft < 60)
		{
			mulColor *= Projectile.timeLeft / 60f;
		}
		Vector2 bossCenter = OwnerBoss.Center + new Vector2(0, -120 * (OwnerBoss.scale - 0.3f));
		var flowPosList = new List<Vector2>();
		Vector2 flowPos = Absorbee.Center;
		Vector2 flowVelocity = Vector2.Normalize(bossCenter - Absorbee.Center).RotatedBy(MathF.Sin(timeValue + Absorbee.whoAmI) * 0.5f) * 6;
		for (int i = 0; i < 600; i++)
		{
			flowPosList.Add(flowPos);
			flowPos += flowVelocity;
			Vector2 toTarget = bossCenter - flowPos - flowVelocity;
			if (!Main.gamePaused)
			{
				if (Main.rand.NextBool(100) && toTarget.Length() >= 200 * OwnerBoss.scale)
				{
					float addRot = Main.rand.NextFloat(-0.7f, 0.7f);
					Vector2 velocity = flowVelocity;
					var dustVFX = new JellyBallSparkTrail
					{
						Velocity = velocity.RotatedBy(addRot),
						Active = true,
						Visible = true,
						Position = flowPos,
						MaxTime = 120,
						Scale = 2,
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f), -addRot * 0.15f, 0 },
					};
					Ins.VFXManager.Add(dustVFX);
				}
			}
			if (toTarget.Length() >= 20)
			{
				flowVelocity = Vector2.Normalize(toTarget) * 0.05f * 6f + flowVelocity * 0.95f;
			}
			else
			{
				break;
			}
		}
		Color drawColor = new Color(0.05f, 0.03f, 1f, 0f) * mulColor;
		Color drawColor_dark = new Color(1f, 1f, 1f, 1f) * mulColor * 0.3f;
		float width = 15f;
		if (Absorbee.type == ModContent.NPCType<GiantJellyBall>())
		{
			width = 30f;
		}
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
		if (powerFlow.Count > 3)
		{
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_9_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, powerFlow_dark.ToArray(), 0, powerFlow_dark.Count - 2);

			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_9.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, powerFlow.ToArray(), 0, powerFlow.Count - 2);
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
				powerCircleLarge.Add(Absorbee.Center + radius - Main.screenPosition, drawColor * mulColor2 * 0.5f, new Vector3(i / 25f - timeValue * 0.25f + Absorbee.whoAmI, 0, 0));
				powerCircleLarge.Add(Absorbee.Center + radius2 - Main.screenPosition, drawColor * mulColor2 * 0.7f, new Vector3(i / 25f - timeValue * 0.25f + Absorbee.whoAmI, 1, 0));
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
