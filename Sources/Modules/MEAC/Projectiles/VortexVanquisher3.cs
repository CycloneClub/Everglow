using Terraria.Audio;
using Terraria.GameContent.Shaders;

namespace Everglow.MEAC.Projectiles;

public class VortexVanquisher3 : ModProjectile
{
	public override string Texture => "Everglow/MEAC/Projectiles/VortexVanquisher";

	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 3;
	}

	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 36;
		Projectile.netImportant = true;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 150000;
		Projectile.extraUpdates = 10;
		Projectile.tileCollide = false;
	}

	private bool crash = false;
	private Vector2 startVelocity = Vector2.Zero;

	public override void AI()
	{
		Lighting.AddLight(Projectile.Center, 0.9f, 0.6f, 0f);
		Projectile.friendly = true;
		Projectile.rotation = Projectile.velocity.ToRotation() + 0.7854f;
		if (startVelocity == Vector2.Zero)
		{
			startVelocity = Vector2.Normalize(Projectile.velocity);
		}

		if (!crash)
		{
			if (Projectile.extraUpdates < 50)
			{
				Projectile.extraUpdates++;
			}

			if (Collision.SolidCollision(Projectile.Center - startVelocity * 12, 0, 0))
			{
				crash = true;
				Projectile.timeLeft = 70;
				Projectile.extraUpdates = 2;

				// ScreenShaker Gsplayer = Main.player[Projectile.owner].GetModPlayer<ScreenShaker>();

				// Gsplayer.FlyCamPosition = new Vector2(0, 28).RotatedByRandom(6.283);
				// 震动示例
				ShakerManager.AddShaker(Projectile.Center - startVelocity * 12, Projectile.velocity, 6, 0.8f, 16, 0.9f, 0.8f, 30);
				SoundEngine.PlaySound(SoundID.NPCHit4);
				for (int g = 0; g < 12; g++)
				{
					Vector2 newVelocity = new Vector2(-Main.rand.NextFloat(35f, 44f), 0).RotatedBy(Projectile.rotation - MathHelper.PiOver4 + Main.rand.NextFloat(-1.4f, 1.4f));
					var somg = new VortexVanquisherGlowingSmogLine_front
					{
						velocity = newVelocity,
						Active = true,
						Visible = true,
						position = Projectile.Center + new Vector2(10, 0).RotatedBy(Projectile.rotation - MathHelper.PiOver4) - newVelocity * 0.2f,
						maxTime = Main.rand.Next(15, 38),
						scale = Main.rand.NextFloat(40f, 60f),
						ai = new float[] { 0, 0 },
					};
					Ins.VFXManager.Add(somg);
				}
			}
		}
		else
		{
			Projectile.velocity *= 0.4f;
		}
		if (Projectile.timeLeft <= 9 && Projectile.timeLeft >= 0 && Projectile.timeLeft % 3 == 0)
		{
			for (int x = 0; x < 12; x++)
			{
				float X = (float)Math.Sqrt(Main.rand.NextFloat(0, 0.5f));
				float Y = Main.rand.NextFloat(-2, 0.3f);
				Vector2 v0 = RotByPro(new Vector2(X * Math.Sign(Main.rand.NextFloat(-1, 1)) * 1, Y * 103));
				int k = Dust.NewDust(Projectile.Center + v0 - new Vector2(4), 0, 0, DustID.GoldFlame, 0, 0, 0, default, Main.rand.NextFloat(0.4f, 1.2f));
				Main.dust[k].noGravity = true;
			}
		}

		mainVec = Projectile.velocity;
		ProduceWaterRipples(new Vector2(mainVec.Length(), 30));
	}

	private Vector2 mainVec = Vector2.One;

	private void ProduceWaterRipples(Vector2 beamDims)
	{
		mainVec = Projectile.velocity;
		var shaderData = (WaterShaderData)Terraria.Graphics.Effects.Filters.Scene["WaterDistortion"].GetShader();
		float waveSine = 1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
		Vector2 ripplePos = Projectile.Center + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(mainVec.ToRotation());
		Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
		shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, mainVec.ToRotation());
	}

	public Vector2 RotByPro(Vector2 orig)
	{
		return orig.RotatedBy(Projectile.rotation - Math.PI * 0.75);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		Main.spriteBatch.Draw(tex, Projectile.Center - startVelocity * 90 - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.scale, 0, 0);
		Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Everglow/MEAC/Projectiles/VortexVanquisherGlow").Value, Projectile.Center - startVelocity * 90 - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, tex.Size() / 2, Projectile.scale, 0, 0);
		if (Projectile.timeLeft < 10)
		{
			for (int x = 0; x < 10 - Projectile.timeLeft; x++)
			{
				Main.spriteBatch.Draw(tex, Projectile.Center - startVelocity * 90 - Main.screenPosition, null, new Color(1f, 1f, 1f, 0f), Projectile.rotation, tex.Size() / 2, Projectile.scale, 0, 0);
			}
		}
		return false;
	}
}
