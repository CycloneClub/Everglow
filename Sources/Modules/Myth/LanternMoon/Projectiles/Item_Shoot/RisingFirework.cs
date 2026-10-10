using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.Audio;

namespace Everglow.Myth.LanternMoon.Projectiles.Item_Shoot;

public class RisingFirework : ModProjectile
{
	public override string Texture => "Everglow/Myth/UIImages/VisualTextures/DarkGrey";

	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 600;
		Projectile.penetrate = -1;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 90;
	}

	public bool MoveSight = true;
	public int TimeToKill = -1;

	public override void AI()
	{
		TimeToKill--;
		Projectile.velocity *= 0.99f;
		Projectile.velocity.Y += 0.15f;
		if (Projectile.velocity.Y >= -3)
		{
			if (TimeToKill < 0)
			{
				TimeToKill = 90;
			}
		}
		if (MoveSight)
		{
			Player player = Main.player[Projectile.owner];
			FireworkVisitor fireworkVisitor = player.GetModPlayer<FireworkVisitor>();
			if (fireworkVisitor != null)
			{
				fireworkVisitor.BestFireworkView += (Projectile.Center + new Vector2(0, 200) - player.Center - fireworkVisitor.BestFireworkView) * 0.4f;
			}
		}
		if (TimeToKill > 0)
		{
			Projectile.velocity *= 0.8f;
			if (TimeToKill == 80)
			{
				SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/FireworkExplosion"), Projectile.Center);
				var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<Firework6Inches>(), 50, 0f, Projectile.owner, 0, 0);
				if (p != null)
				{
					var fireworkProjectile = p.ModProjectile as FireworkProjectile;
					if (fireworkProjectile != null)
					{
						fireworkProjectile.MoveSight = MoveSight;
					}
				}
				GenerateFire(120);
				GenerateSpark(120);
			}
			if (TimeToKill == 1)
			{
				Projectile.Kill();
			}
		}
	}

	public void GenerateSmog(int frequency)
	{
		float mulVelocity = 1f;
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var smog = new FireSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 3,
				MaxTime = Main.rand.Next(37, 45),
				Scale = Main.rand.NextFloat(20f, 35f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(smog);
		}
	}

	public void GenerateFire(int frequency)
	{
		float mulVelocity = 1f;
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 1f)).RotatedByRandom(MathHelper.TwoPi);
			var fire = new FireDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + newVelocity * 3,
				MaxTime = Main.rand.Next(9, 25),
				Scale = Main.rand.NextFloat(7f, 15f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(fire);
		}
	}

	public void GenerateSpark(int frequency)
	{
		float mulVelocity = 1f;
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 2f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new FireSparkDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - Projectile.velocity + newVelocity * 3,
				MaxTime = Main.rand.Next(17, 25),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(0.1f, 17.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.13f, 0.13f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Projectile.velocity *= Main.rand.NextFloat(0.3f, 0.8f);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		DrawTrail(new Color(1f, 0.7f, 0.4f, 0));
	}

	private void DrawTrail(Color c0)
	{
		float k0 = Projectile.timeLeft / 60f;
		var bars = new List<Vertex2D>();

		int length = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			length++;
		}
		for (int i = 1; i < length; ++i)
		{
			if (i % 24 != 1 && i < length - 24 && i > 8)
			{
				continue;
			}
			float width = 108 - k0 * 36;
			width *= Projectile.ai[0] * 0.3f;
			if (i < 10)
			{
				width *= i / 10f;
			}

			if (Projectile.ai[0] == 3)
			{
				width *= 0.5f;
			}

			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)length;
			float toKillValue = 0;
			if (TimeToKill > 0)
			{
				toKillValue = 1 - TimeToKill / 90f;
			}
			toKillValue *= 2;
			float x0 = factor * 1.6f + (float)(Main.timeForVisualEffects / 70d) + MathF.Sin(Projectile.whoAmI);
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width * (1 - factor) + new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition, c0, new Vector3(x0, 1, 1 - factor - toKillValue)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width * (1 - factor) + new Vector2(Projectile.width / 2f, Projectile.height / 2f) - Main.screenPosition, c0, new Vector3(x0, 0, 1 - factor - toKillValue)));
		}

		if (bars.Count > 3)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Trail_1.Value;
			Effect effect = ModAsset.FireworkTrailStyle1.Value;
			effect.Parameters["uPerlin"].SetValue(ModAsset.CorruptDustFadePowderII.Value);
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Main.GameViewMatrix.TransformationMatrix;
			effect.Parameters["uTransform"].SetValue(model * projection);
			effect.CurrentTechnique.Passes["Test"].Apply();
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.LightPoint.Value;
		}
	}
}
