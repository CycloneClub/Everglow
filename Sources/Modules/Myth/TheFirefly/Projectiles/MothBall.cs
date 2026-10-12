using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Myth.TheFirefly.NPCs.Bosses;
using Everglow.Myth.TheFirefly.VFXs;
using Terraria.Audio;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class MothBall : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	private float subscale = 0f;

	public override void SetDefaults()
	{
		Projectile.width = 32;
		Projectile.height = 32;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 300;
		Projectile.tileCollide = false;
	}

	public void GenerateBranchedLighting()
	{
		var lightning = new BranchedLightning(100f, 9f, Projectile.position, Main.rand.NextVector2Unit().ToRotation(), 25f, 0);
		Ins.VFXManager.Add(lightning);
	}

	public void GenerateLightingBolt()
	{
		float size = Main.rand.NextFloat(18f, Main.rand.NextFloat(20f, 40f));
		Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(9f, 30f)).RotatedByRandom(MathHelper.TwoPi);
		var electric = new MothBallCurrent
		{
			Velocity = afterVelocity + Projectile.velocity,
			Active = true,
			Visible = true,
			Position = Projectile.Center,
			MaxTime = Main.rand.Next(42, 90),
			Scale = size,
			ProjectileOwner = Projectile.whoAmI,
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.6f), 2 },
		};
		Ins.VFXManager.Add(electric);
	}

	public override void AI()
	{
		Player player = Main.player[Player.FindClosest(Projectile.position, Projectile.width, Projectile.height)];
		if (Projectile.timeLeft > 240)
		{
			subscale += 1f;
		}

		if (Projectile.timeLeft is <= 240 and >= 60)
		{
			subscale = 60 + (float)(10 * Math.Sin((Projectile.timeLeft - 60) / 60d * Math.PI));
		}

		if (Projectile.timeLeft < 60 && subscale > 0.5f)
		{
			subscale -= 1f;
		}

		if (Projectile.timeLeft < 50)
		{
			Projectile.velocity *= 0.95f;
			Projectile.scale *= 0.97f;

			if (Projectile.timeLeft > 10 && Main.rand.NextFloat() < (0.15 + 0.2 * (50 - Projectile.timeLeft) / 40))
			{
				GenerateBranchedLighting();
			}
			else
			{
				GenerateLightingBolt();
			}
		}
		else
		{
			float speed = MathHelper.Clamp((300 - Projectile.timeLeft) * 0.1f, 0, 30);
			speed *= MathHelper.Clamp(Vector2.Distance(Projectile.Center, player.Center) / 300, 1, 2f);
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(player.Center) * speed, 0.015f);
		}
		if (Projectile.frame >= 30)
		{
			Projectile.frame = 0;
		}
		else
		{
			Projectile.frame++;
		}
		if (Projectile.timeLeft == 185)
		{
			SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/PowerAccumulate"), Projectile.Center);
		}
	}

	public override void OnKill(int timeLeft)
	{
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MothBallExplosion>(), 50, 3, Projectile.owner, 60f);
		if (Main.masterMode)
		{
			for (int h = 0; h < 6; h++)
			{
				Vector2 v0 = new Vector2(0, 8f).RotatedBy(Main.timeForVisualEffects + h / 3d * Math.PI);
				Vector2 v1 = new Vector2(0, 8f).RotatedBy(Main.timeForVisualEffects + h / 3d * Math.PI + 0.2);
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, v0, ModContent.ProjectileType<MissileProjHostile>(), Projectile.damage, 3, Projectile.owner, 4f);
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, v1, ModContent.ProjectileType<MissileProjHostile>(), Projectile.damage, 3, Projectile.owner, 4f);
			}
		}
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			for (int y = 0; y < 10; y++)
			{
				for (int i = 0; i < 5; i++)
				{
					var npc = NPC.NewNPCDirect(Projectile.GetSource_FromAI(), Projectile.Center, ModContent.NPCType<SummonedButterfly>());
					npc.velocity = Main.rand.NextVector2Unit() * Main.rand.Next(4, 12);
					npc.netUpdate2 = true;
				}
				for (int i = 0; i < 4; i++)
				{
					var npc = NPC.NewNPCDirect(Projectile.GetSource_FromAI(), Projectile.Center, ModContent.NPCType<SummonedButterfly>());
					npc.velocity = Main.rand.NextVector2Unit() * Main.rand.Next(2, 5);
					npc.netUpdate2 = true;
				}
			}

			int player = Player.FindClosest(Projectile.Center, 1000, 1000);
			float addRot = 0;
			if (player is >= 0 and < 255)
			{
				addRot = Projectile.DirectionTo(Main.player[player].Center).ToRotation();
			}

			for (int h = 0; h < 36; h++)
			{
				if (h % 6 < 3)
				{
					Vector2 v = new Vector2(0, 12f).RotatedBy(h * MathHelper.TwoPi / 36f + addRot);
					Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + v, v, ModContent.ProjectileType<BlueMissil>(), Projectile.damage, 0f, Main.myPlayer, 2);
				}
			}
		}

		// base.OnKill(timeLeft);
		// Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.CorruptMoth.FruitBomb>(), 0, 0f, Main.myPlayer, 1);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D light = ModAsset.CorruptLight.Value;
		int frameX = Projectile.frame % 6;
		int frameY = (Projectile.frame - frameX) / 6;
		int frameSideX = 270;
		int frameSideY = 290;
		Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center - Main.screenPosition, new Rectangle(frameX * frameSideX, frameY * frameSideY + 10, 270, 270), new Color(1f, 1f, 1f, 0), Projectile.rotation, new Vector2(135f), Projectile.scale * subscale / 60f, SpriteEffects.None, 0f);

		if (Projectile.timeLeft < 60)
		{
			Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, light.Size() / 2f, (60 - Projectile.timeLeft) / 30f, SpriteEffects.None, 0);
		}

		float range = 720f;
		if (Projectile.timeLeft < 100)
		{
			range += (100 - Projectile.timeLeft) * 16;
		}
		for (int k = 0; k < 9; k++)
		{
			DrawCurrents(k - (float)Main.time * 0.03f, k, range);
		}

		return false;
	}

	public void DrawCurrents(float addRot, float randomSeed, float startLength = 180f)
	{
		List<Vector2> current = new List<Vector2>();
		Vector2 start = new Vector2(0, startLength);
		float maxTime = 17f;
		if (Projectile.timeLeft < 100)
		{
			maxTime += (100 - Projectile.timeLeft) * 0.1f;
		}
		for (int t = 1; t < maxTime; t++)
		{
			start = start * 0.82f + new Vector2(0, 20) * 0.18f;
			Vector2 v0 = start.RotatedBy(-t * t / 60f + addRot);
			current.Add(v0);
		}
		current = GraphicsUtils.CatmullRom(current);

		float length = current.Count;
		float timeValue = (float)Main.timeForVisualEffects * 0.02f;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int t = (int)length - 1; t >= 0; t--)
		{
			float value = t / length;
			Vector2 normalize = Vector2.Normalize(current[t].RotatedBy(Math.PI * 0.5));
			if (t != 0)
			{
				normalize = Vector2.Normalize(current[t] - current[t - 1]).RotatedBy(-Math.PI * 0.5);
			}
			float width = 50 + randomSeed * 2;
			normalize *= width;
			float colorValue = 0f;
			if (Projectile.timeLeft < 100)
			{
				colorValue = value * value * (100 - Projectile.timeLeft) / 50f;
			}
			bars.Add(current[t] + Projectile.Center + normalize, new Color(0, 200, 255, 0) * colorValue, new Vector3(1 - value + timeValue + randomSeed * 0.13f, 0, value));
			bars.Add(current[t] + Projectile.Center - normalize, new Color(0, 200, 255, 0) * colorValue, new Vector3(1 - value + timeValue + randomSeed * 0.13f, 1, value));
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
		Effect effect = Commons.ModAsset.StabSwordEffect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uProcession"].SetValue(0.5f);
		effect.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_2_thick.Value;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
	}
}
