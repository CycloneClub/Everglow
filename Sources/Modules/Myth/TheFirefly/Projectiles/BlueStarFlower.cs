using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Myth.TheFirefly.NPCs.Bosses;
using Everglow.Myth.TheFirefly.VFXs;
using Terraria.Audio;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class BlueStarFlower : ModProjectile
{
	public override string Texture => "Everglow/Myth/TheFirefly/Projectiles/CorruptLight";

	public override void SetDefaults()
	{
		Projectile.width = 8;
		Projectile.height = 8;
		Projectile.aiStyle = -1;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.extraUpdates = 2;
		Projectile.timeLeft = 240;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 0;
	}

	public NPC OwnerCorruptMoth;

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color?(new Color(255, 255, 255, 0));
	}

	public override void AI()
	{
		if (Projectile.scale < 1)
		{
			Projectile.scale += 0.02f;
		}
		if (OwnerCorruptMoth != null)
		{
			if (OwnerCorruptMoth.active && OwnerCorruptMoth.type == ModContent.NPCType<CorruptMoth>())
			{
				Vector2 aimCenter = OwnerCorruptMoth.Center + new Vector2(100 * OwnerCorruptMoth.spriteDirection, -80).RotatedBy(OwnerCorruptMoth.rotation * OwnerCorruptMoth.spriteDirection);
				Projectile.Center = aimCenter * 0.2f + Projectile.Center * 0.8f;
			}
		}
		Lighting.AddLight(Projectile.Center, 0, 0.4f * Projectile.scale, 0.9f * Projectile.scale);
	}

	public void GenerateSmog(int Frequency)
	{
		float mulVelocity = 2;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new FireSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 4,
				MaxTime = Main.rand.Next(37, 85),
				Scale = Main.rand.NextFloat(12f, 27f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public void GenerateFire(int Frequency)
	{
		float mulVelocity = 2;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var fire = new MothBlueFireDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 4,
				MaxTime = Main.rand.Next(9, 55),
				Scale = Main.rand.NextFloat(12f, 27f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(fire);
		}
	}

	public void GenerateSpark(int Frequency)
	{
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 17.6f)).RotatedByRandom(MathHelper.TwoPi);
			var smog = new MothShimmerScaleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				Coord = new Vector2(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f)),
				MaxTime = Main.rand.Next(20, 85),
				Scale = Main.rand.NextFloat(0.4f, 8.4f),
				Rotation = Main.rand.NextFloat(6.283f),
				Rotation2 = Main.rand.NextFloat(6.283f),
				Omega = Main.rand.NextFloat(-30f, 30f),
				Phi = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(-0.005f, 0.005f) },
			};
			Ins.VFXManager.Add(smog);
		}
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(new SoundStyle("Everglow/Myth/Sounds/Crystal_Burst_Normal").WithVolumeScale(Projectile.ai[0] / 20f + 0.2f), Projectile.Center);
		GenerateSmog(12);
		GenerateFire(20);
		GenerateSpark(12);
		int style = Main.rand.Next(3);
		float r = Main.rand.NextFloat() * 10;
		if (style == 0)
		{
			int c = Projectile.ai[0] == 1 ? 6 : 5;
			for (int i = 0; i < c; i++)
			{
				for (int j = -3; j <= 3; j++)
				{
					Vector2 v = new Vector2(0.1f + j * 0.11f, 0).RotatedBy(j * 0.15f + i * MathHelper.TwoPi / c + r);
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, v, ModContent.ProjectileType<BlackCorruptRain3>(), Projectile.damage / 6, 0f, Main.myPlayer, Main.rand.NextFloat(MathF.PI * 2)); // Originally: NPC.damage / 5
				}
			}
		}
		if (style == 1)
		{
			if (Projectile.ai[0] == 1)
			{
				for (int i = 0; i < 40; i++)
				{
					Vector2 v = new Vector2(0.1f + i % 5 / 16f, 0).RotatedBy(i * MathHelper.TwoPi / 40 + r);
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, v, ModContent.ProjectileType<BlackCorruptRain3>(), Projectile.damage / 6, 0f, Main.myPlayer, Main.rand.NextFloat(MathF.PI * 2));
				}
			}
			else
			{
				for (int i = 0; i < 30; i++)
				{
					Vector2 v = new Vector2(0.1f + i % 5 / 16f, 0).RotatedBy(i * MathHelper.TwoPi / 30 + r);
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, v, ModContent.ProjectileType<BlackCorruptRain3>(), Projectile.damage / 6, 0f, Main.myPlayer, Main.rand.NextFloat(MathF.PI * 2));
				}
			}
		}
		if (style == 2)
		{
			int c = Projectile.ai[0] == 1 ? 60 : 50;
			for (int i = 0; i < c; i++)
			{
				Vector2 v = new Vector2(0.18f + (float)Math.Sin(i * MathHelper.TwoPi / 10) * 0.17f, 0).RotatedBy(i * MathHelper.TwoPi / c + r);
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, v, ModContent.ProjectileType<BlackCorruptRain3>(), Projectile.damage / 6, 0f, Main.myPlayer, Main.rand.NextFloat(MathF.PI * 2));
			}
		}
		if (Main.masterMode)
		{
			// for (int i = 0; i < 4; i++)
			// {
			// Vector2 v = new Vector2(4, -2).RotatedBy(i * 0.3f) * (9 - i) / 6f;
			// Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center - v, v, ModContent.ProjectileType<MothMiddleBullet>(), Projectile.damage / 3, 0f, Main.myPlayer);
			// p.timeLeft = Main.rand.Next(1580, 1620);
			// }
			// for (int i = 0; i < 4; i++)
			// {
			// Vector2 v = new Vector2(-4, -2).RotatedBy(-i * 0.3f) * (9 - i) / 6f;
			// Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center - v, v, ModContent.ProjectileType<MothMiddleBullet>(), Projectile.damage / 3, 0f, Main.myPlayer);
			// p.timeLeft = Main.rand.Next(1580, 1620);
			// }
		}
	}

	public override void PostDraw(Color lightColor)
	{
		Vector2 center = Projectile.Center - Main.screenPosition;
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		float range = 90;
		if (Projectile.timeLeft < 30)
		{
			range = Projectile.timeLeft / 30f;
			range *= range;
			range *= 90f;
		}
		float waveValue = (float)(Math.Sin(Main.time * 0.15f + Projectile.whoAmI) + Math.Sin(Main.time * 0.7f + Projectile.whoAmI * 7) * 0.25) * 0.4f + 1;
		float waveValue2 = (float)(Math.Sin(Main.time * 0.23f + Projectile.whoAmI) * 0.4 + Math.Sin(Main.time * 0.49f + Projectile.whoAmI * 3)) * 0.4f + 1;
		Main.spriteBatch.Draw(star, center, null, new Color(255, 255, 255, 0), MathHelper.PiOver2, star.Size() / 2f, new Vector2(waveValue * range / 90f, 1) * Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, center, null, new Color(255, 255, 255, 0), 0, star.Size() / 2f, new Vector2(waveValue2 * range / 90f, 0.5f) * Projectile.scale, SpriteEffects.None, 0);

		var circle = new List<Vertex2D>();
		for (int h = 0; h < 20; h += 1)
		{
			float timeValue = (float)Main.time * 0.002f;
			float addTime = 240 - Projectile.timeLeft;
			addTime /= 240f;
			addTime *= addTime;
			addTime *= 0.9f;
			timeValue += addTime * (MathF.Sin(h) * 0.4f + 1);
			Vector2 front = new Vector2(0, range).RotatedBy(h * MathHelper.TwoPi / 20f) * Projectile.scale;
			Vector2 right = new Vector2(35, 0).RotatedBy(h * MathHelper.TwoPi / 20f) * Projectile.scale;
			circle.Add(new Vertex2D(center + right, Color.Transparent, new Vector3(h / 20f + timeValue, 1, 0)));
			circle.Add(new Vertex2D(center - right, Color.Transparent, new Vector3(h / 20f + timeValue, 1, 0)));

			circle.Add(new Vertex2D(center + right, Color.White, new Vector3((h + 8) / 20f + timeValue, 0, 0)));
			circle.Add(new Vertex2D(center - right, Color.White, new Vector3((h + 8) / 20f + timeValue, 1, 0)));

			circle.Add(new Vertex2D(center - front + right, Color.Transparent, new Vector3((h + 1) / 20f + timeValue, 0, 0)));
			circle.Add(new Vertex2D(center - front - right, Color.Transparent, new Vector3((h + 1) / 20f + timeValue, 1, 0)));
		}
		Main.graphics.graphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_0_black.Value;
		if (circle.Count > 2)
		{
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}

		circle = new List<Vertex2D>();
		for (int h = 0; h < 20; h += 1)
		{
			float timeValue = (float)Main.time * 0.002f;
			float addTime = 240 - Projectile.timeLeft;
			addTime /= 240f;
			addTime *= addTime;
			addTime *= 0.9f;
			timeValue += addTime * (MathF.Sin(h) * 0.4f + 1);
			Vector2 front = new Vector2(0, range).RotatedBy(h * MathHelper.TwoPi / 20f) * Projectile.scale;
			Vector2 right = new Vector2(35, 0).RotatedBy(h * MathHelper.TwoPi / 20f) * Projectile.scale;
			circle.Add(new Vertex2D(center + right, Color.Transparent, new Vector3(h / 20f + timeValue, 1, 0)));
			circle.Add(new Vertex2D(center - right, Color.Transparent, new Vector3(h / 20f + timeValue, 1, 0)));

			circle.Add(new Vertex2D(center + right, new Color(0, 0.4f, 1f, 0), new Vector3((h + 8) / 20f + timeValue, 0, 0)));
			circle.Add(new Vertex2D(center - right, new Color(0, 0.4f, 1f, 0), new Vector3((h + 8) / 20f + timeValue, 1, 0)));

			circle.Add(new Vertex2D(center - front + right, Color.Transparent, new Vector3((h + 1) / 20f + timeValue, 0, 0)));
			circle.Add(new Vertex2D(center - front - right, Color.Transparent, new Vector3((h + 1) / 20f + timeValue, 1, 0)));
		}

		Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_0.Value;
		if (circle.Count > 2)
		{
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circle.ToArray(), 0, circle.Count - 2);
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
}
