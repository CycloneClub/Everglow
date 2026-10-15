using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class IntroductiontoThermalElectricity_Ball : ModProjectile
{
	public int Timer = 0;

	public int TimeToKill = -1;

	public float HitRange = 25;

	public int TimeToKillMax = 45;

	public override string LocalizationCategory => LocalizationUtils.Categories.MagicProjectiles;

	public override void OnSpawn(IEntitySource source) => base.OnSpawn(source);

	public override void SetDefaults()
	{
		Projectile.friendly = true;
		Projectile.timeLeft = 900;
		Projectile.tileCollide = true;
		Projectile.penetrate = -1;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.aiStyle = -1;
		Projectile.magic = true;
		Projectile.tileCollide = true;

		Timer = 0;
		TimeToKill = -1;
		HitRange = 25;
	}

	public void HitTarget()
	{
		switch (Main.rand.Next(2))
		{
			case 0:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.Introductiontothermalelectricity_explosion_1_Mod), Projectile.Center);
				break;
			case 1:
				SoundEngine.PlaySound(new SoundStyle(ModAsset.Introductiontothermalelectricity_explosion_2_Mod), Projectile.Center);
				break;
		}
		TimeToKill = TimeToKillMax;
		for (int k = 0; k < 40; k++)
		{
			var dust = new IntroductiontoThermalElectricity_Plasma
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(2f, 5f)).RotatedByRandom(MathHelper.TwoPi),
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(0, 10).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = Main.rand.NextFloat(25f, 55f),
				Scale = Main.rand.NextFloat(1f, 3f),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			};
			Ins.VFXManager.Add(dust);
		}
		for (int k = 0; k < 12; k++)
		{
			var dust = new IntroductiontoThermalElectricity_Plasma_Trail
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi),
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.NextFloat(55f, 85f),
				Scale = 2f,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			};
			Ins.VFXManager.Add(dust);
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if ((targetHitbox.Center() - Projectile.Center).Length() < targetHitbox.Width / 2f + HitRange)
		{
			return true;
		}
		return base.Colliding(projHitbox, targetHitbox);
	}

	public override void AI()
	{
		Timer++;
		if (Projectile.wet)
		{
			if (TimeToKill < 0)
			{
				HitTarget();
			}
		}
		if (TimeToKill > 0)
		{
			TimeToKill--;
			float addRangeValue = (TimeToKillMax - TimeToKill) / (float)TimeToKillMax;
			addRangeValue = MathF.Pow(addRangeValue, 0.5f);
			HitRange = 25 + addRangeValue * 100;
			Projectile.velocity *= 0;
			if (TimeToKill <= 0)
			{
				Projectile.Kill();
			}
			if (TimeToKill < 10)
			{
				Projectile.friendly = false;
			}
		}
		else
		{
			if (Main.rand.NextBool())
			{
				return;
			}
			if (Main.rand.NextBool(3))
			{
				var dust = new IntroductiontoThermalElectricity_Plasma_Trail
				{
					Velocity = Projectile.velocity.RotatedByRandom(0.2f),
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(0, 10).RotatedByRandom(MathHelper.TwoPi),
					MaxTime = Main.rand.NextFloat(25f, 55f),
					Scale = Main.rand.NextFloat(1f, 2f),
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				};
				Ins.VFXManager.Add(dust);
			}
			else
			{
				var dust = new IntroductiontoThermalElectricity_Plasma
				{
					Velocity = Projectile.velocity.RotatedByRandom(0.2f),
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(0, 10).RotatedByRandom(MathHelper.TwoPi),
					MaxTime = Main.rand.NextFloat(25f, 55f),
					Scale = Main.rand.NextFloat(1f, 2f),
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				};
				Ins.VFXManager.Add(dust);
			}
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (TimeToKill < 0)
		{
			HitTarget();
		}
		base.OnHitNPC(target, hit, damageDone);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (TimeToKill < 0)
		{
			Projectile.tileCollide = false;
			HitTarget();
		}
		return false;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Texture2D tex_bloom = Commons.ModAsset.LightPoint.Value;
		int frameCurrent = (int)((Timer / 3f) % 11);
		Rectangle frame = new Rectangle(0, frameCurrent * 56, 56, 56);
		Color drawColor = new Color(1f, 1f, 1f, 0.75f);
		float lerpColorValue = 0.5f + MathF.Sin(Timer);
		Color ringColor = Color.Lerp(new Color(255, 109, 91, 0), new Color(56, 255, 255, 0), lerpColorValue);
		if (Timer % 7 == 1)
		{
			ringColor = new Color(1f, 1f, 1f, 0);
		}
		Color darkColor = Color.White * 0.4f;
		float environmentLightValue = 1f;
		float timeToKillValue = TimeToKill / 10f;
		if (TimeToKill < 10 && TimeToKill >= 0)
		{
			ringColor *= timeToKillValue;
			darkColor *= timeToKillValue;
			Projectile.scale = timeToKillValue;
			environmentLightValue *= timeToKillValue;
		}
		Vector2 drawPos = Projectile.Center - Main.screenPosition;
		Main.EntitySpriteDraw(tex_bloom, drawPos, null, ringColor, Projectile.rotation, tex_bloom.Size() * 0.5f, Projectile.scale * 2, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(tex, drawPos, frame, drawColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		if (frameCurrent != 3 && frameCurrent != 7)
		{
			Lighting.AddLight(Projectile.Center, ringColor.ToVector3() * environmentLightValue);
		}
		if (TimeToKill < 0)
		{
			return false;
		}

		float ringRange = HitRange + 5 * MathF.Sin(Timer * 0.15f);
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i <= 60; i++)
		{
			float rot = i / 60f * MathHelper.TwoPi;
			bars.Add(drawPos + new Vector2(0, ringRange).RotatedBy(rot), ringColor, new Vector3(i / 60f, Timer / 120f, 0));
			bars.Add(drawPos + new Vector2(0, Math.Max(ringRange - 15, 0)).RotatedBy(rot), ringColor * 0, new Vector3(i / 60f, 0.1f + Timer / 120f, 0));
			Lighting.AddLight(Projectile.Center + new Vector2(0, ringRange).RotatedBy(rot), ringColor.ToVector3() * environmentLightValue * 0.3f);
		}

		float outerRingThick = MathF.Pow(TimeToKill / (float)TimeToKillMax, 2) * 80;
		float outerRingBoundVagueValue = 1 - TimeToKill / (float)TimeToKillMax;
		outerRingBoundVagueValue = MathF.Sin(outerRingBoundVagueValue * MathHelper.Pi - MathHelper.PiOver2) * 0.5f + 0.5f;
		outerRingBoundVagueValue *= 0.5f;
		List<Vertex2D> bars_outerRing = new List<Vertex2D>();
		List<Vertex2D> bars_outerRing_dark = new List<Vertex2D>();
		for (int i = 0; i <= 60; i++)
		{
			float rot = i / 60f * MathHelper.TwoPi;
			bars_outerRing.Add(drawPos + new Vector2(0, ringRange).RotatedBy(rot), ringColor, new Vector3(i / 60f, 0.5f, 0));
			bars_outerRing.Add(drawPos + new Vector2(0, Math.Max(ringRange - outerRingThick, 0)).RotatedBy(rot), ringColor, new Vector3(i / 60f, 0.5f + outerRingBoundVagueValue, 0));

			bars_outerRing_dark.Add(drawPos + new Vector2(0, ringRange).RotatedBy(rot), darkColor, new Vector3(i / 60f, 0.5f, 0));
			bars_outerRing_dark.Add(drawPos + new Vector2(0, Math.Max(ringRange - outerRingThick * 1.4f, 0)).RotatedBy(rot), darkColor * 0, new Vector3(i / 60f, 1, 0));
		}
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		if (bars_outerRing_dark.Count > 2)
		{
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail_black.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_outerRing_dark.ToArray(), 0, bars_outerRing_dark.Count - 2);
		}

		if (bars.Count > 2)
		{
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_hiveNet.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
		if (bars_outerRing.Count > 2)
		{
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Trail.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars_outerRing.ToArray(), 0, bars_outerRing.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
	}
}
