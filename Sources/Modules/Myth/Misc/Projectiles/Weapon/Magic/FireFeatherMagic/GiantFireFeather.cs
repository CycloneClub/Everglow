using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.Audio;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Magic.FireFeatherMagic;

public class GiantFireFeather : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 40;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 360;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.extraUpdates = 3;
		Projectile.localNPCHitCooldown = 2;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 80;
	}

	internal int TimeTokill = -1;

	public override void AI()
	{
		if (TimeTokill >= 0 && TimeTokill <= 2)
		{
			Projectile.Kill();
		}

		if (TimeTokill <= 80 && TimeTokill > 0)
		{
			Projectile.velocity = Projectile.oldVelocity;
		}

		TimeTokill--;
		if (TimeTokill >= 0)
		{
			if (TimeTokill < 10)
			{
				Projectile.damage = 0;
				Projectile.friendly = false;
			}
			Projectile.velocity *= 0f;
		}
		else
		{
			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
			Projectile.tileCollide = Projectile.position.Y >= Main.MouseWorld.Y;
			if (Projectile.Center.X > Main.screenPosition.X - 100 && Projectile.Center.X < Main.screenPosition.X + Main.screenWidth + 100 && Projectile.Center.Y > Main.screenPosition.Y - 100 && Projectile.Center.Y < Main.screenPosition.Y + Main.screenWidth + 100)
			{
				if (Main.rand.NextBool(6))
				{
					Vector2 v = new Vector2(0, Main.rand.NextFloat(0, 2)).RotatedByRandom(MathHelper.TwoPi);
					Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.FireFeather>(), v.X, v.Y, 150, default, Main.rand.NextFloat(0.8f, 1.7f));
				}
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 0.6f)).RotatedByRandom(MathHelper.TwoPi);
				var spark = new FireSparkDust
				{
					Velocity = newVelocity + Projectile.velocity * Main.rand.NextFloat(0.7f, 2.9f),
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * Main.rand.NextFloat(-3f, 2f),
					MaxTime = Main.rand.Next(37, 45),
					Scale = Main.rand.NextFloat(0.1f, 12.0f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
				};
				Ins.VFXManager.Add(spark);
				for (int x = 0; x < 6; x++)
				{
					var spark2 = new FireSparkDust
					{
						Velocity = newVelocity + Projectile.velocity * Main.rand.NextFloat(0.0f, 0.2f),
						Active = true,
						Visible = true,
						Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * Main.rand.NextFloat(-3f, 2f),
						MaxTime = Main.rand.Next(27, 35),
						Scale = Main.rand.NextFloat(0.1f, 12.0f),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
					};
					Ins.VFXManager.Add(spark2);
				}
			}
		}

		if (Projectile.position.X <= 320 || Projectile.position.X >= Main.maxTilesX * 16 - 320)
		{
			Projectile.Kill();
		}
		if (Projectile.position.Y <= 320 || Projectile.position.Y >= Main.maxTilesY * 16 - 320)
		{
			Projectile.Kill();
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Color lightColor)
	{
		if (TimeTokill >= 0)
		{
			float timeValue = (80 - TimeTokill) / 80f;
			DrawTrail(Commons.ModAsset.Trail_2_black_thick.Value, Color.White, 36);
			DrawTrail(Commons.ModAsset.Trail_6.Value, new Color(1f * (1 - timeValue), 0.6f * (1 - timeValue) * (1 - timeValue), 0f, 0f), Math.Max(TimeTokill - 44, 0));
			return;
		}
		else
		{
			DrawTrail(Commons.ModAsset.Trail_2_black_thick.Value, Color.White);
			DrawTrail(Commons.ModAsset.Trail_6.Value, new Color(1f, 0.6f, 0f, 0f));
		}
		SpriteEffects spriteEffects = SpriteEffects.None;
		if (Projectile.spriteDirection == -1)
		{
			spriteEffects = SpriteEffects.FlipHorizontally;
		}

		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
		int frameHeight = texture.Height / Main.projFrames[Projectile.type];
		int startY = frameHeight * Projectile.frame;
		var sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
		Vector2 origin = sourceRectangle.Size() / 2f;
		float offsetX = 20f;
		origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;
		float amount = 1f;
		if (Projectile.timeLeft >= 1040)
		{
			amount = (1080 - Projectile.timeLeft) / 40f;
		}
		Color aimColor = new Color(1f, 1f, 1f, 1f);
		Color drawColor = Color.Lerp(lightColor, aimColor, amount);
		if (Projectile.wet)
		{
			float value = 0.6f;
			if (Projectile.timeLeft < 700)
			{
				value = (Projectile.timeLeft - 100) / 1000f;
			}
			aimColor = new Color(value, value / 12f, 0f, 1f);
			drawColor = aimColor;
		}
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
	}

	public void DrawTrail(Texture2D tex, Color color, float width = 36)
	{
		var c0 = color;
		var bars = new List<Vertex2D>();

		int trueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			trueL++;
		}
		if (TimeTokill > 0)
		{
			trueL = Math.Max(trueL, TimeTokill);
			if (trueL == 0)
			{
				return;
			}
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			float width2 = width;
			if (Projectile.timeLeft <= 40)
			{
				width2 = Projectile.timeLeft * 0.9f;
			}

			if (i < 10)
			{
				width2 *= i / 10f;
			}

			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			if (normalDir == Vector2.zeroVector)
			{
				continue;
			}
			normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
			var factor = i / (float)trueL;
			var w = MathHelper.Lerp(1f, 0.05f, factor);
			float x0 = factor * 0.6f - (float)(Main.timeForVisualEffects / 35d) + 10000;
			if (i > 2)
			{
				if ((Projectile.oldPos[i - 2] - Projectile.oldPos[i - 1]) == Vector2.zeroVector)
				{
					bars.Add(new Vertex2D(Projectile.oldPos[i - 1] + new Vector2(Projectile.width / 2f) - Main.screenPosition + Projectile.velocity, c0, new Vector3(x0, 0.5f, w)));
					bars.Add(new Vertex2D(Projectile.oldPos[i - 1] + new Vector2(Projectile.width / 2f) - Main.screenPosition + Projectile.velocity, c0, new Vector3(x0, 0.5f, w)));
				}
			}
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width2 * (1 - factor) + new Vector2(Projectile.width / 2f) - Main.screenPosition + Projectile.velocity, c0, new Vector3(x0, 1, w)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width2 * (1 - factor) + new Vector2(Projectile.width / 2f) - Main.screenPosition + Projectile.velocity, c0, new Vector3(x0, 0, w)));
		}

		Texture2D t = tex;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Main.graphics.GraphicsDevice.Textures[0] = t;
		if (bars.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		AmmoHit();
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GiantFireFeatherExplosion>(), Projectile.damage, 10, Projectile.owner);
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(BuffID.OnFire3, 600);
		AmmoHit();
	}

	public void AmmoHit()
	{
		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithVolumeScale(0.8f), Projectile.Center);
		if (TimeTokill > 0)
		{
			return;
		}
		Player player = Main.player[Projectile.owner];
		TimeTokill = 80;
		Projectile.tileCollide = false;
		Projectile.friendly = false;
		Projectile.ignoreWater = true;
		Projectile.velocity = Projectile.oldVelocity;
		for (int j = 0; j < 40; j++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(7, 120)).RotatedByRandom(MathHelper.TwoPi);
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.FireFeather>(), v.X, v.Y, 150, default, Main.rand.NextFloat(1.8f, 3.7f));
		}
		GenerateFire(23);
		GenerateSmog(13);
		if (!Projectile.wet)
		{
			GenerateSpark(140);
		}
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<GiantFireFeatherExplosion>(), Projectile.damage, Projectile.knockBack, player.whoAmI, 8);
		Projectile.position -= Projectile.velocity;
	}

	public void GenerateSmog(int Frequency)
	{
		float mulVelocity = 2f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new FireSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 3,
				MaxTime = Main.rand.Next(37, 85),
				Scale = Main.rand.NextFloat(20f, 35f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public void GenerateFire(int Frequency)
	{
		float mulVelocity = 2f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var fire = new FireDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 8,
				MaxTime = Main.rand.Next(9, 55),
				Scale = Main.rand.NextFloat(20f, 130f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(fire);
		}
	}

	public void GenerateSpark(int Frequency)
	{
		float mulVelocity = 4f;
		for (int g = 0; g < Frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new FireSparkDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 3,
				MaxTime = Main.rand.Next(37, 45),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(0.1f, 17.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}
}
