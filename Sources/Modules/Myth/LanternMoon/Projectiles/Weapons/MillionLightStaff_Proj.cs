using Everglow.Commons.Templates.Weapons;
using Everglow.Myth.LanternMoon.VFX;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class MillionLightStaff_Proj : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public int Style;

	public override void SetCustomDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.width = 64;
		Projectile.height = 64;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 360;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 10;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Style = Main.rand.Next(2);
		if (Style == 0)
		{
			switch (Main.rand.Next(4))
			{
				case 0:
					TrailTexture = Commons.ModAsset.Trail_12.Value;
					TrailTextureBlack = Commons.ModAsset.Trail_12_black.Value;
					break;
				case 1:
					TrailTexture = Commons.ModAsset.Trail_16.Value;
					TrailTextureBlack = Commons.ModAsset.Trail_16_black.Value;
					break;
				case 2:
					TrailTexture = Commons.ModAsset.Trail_9.Value;
					TrailTextureBlack = Commons.ModAsset.Trail_9_black.Value;
					break;
				case 3:
					TrailTexture = Commons.ModAsset.Trail_13.Value;
					TrailTextureBlack = Commons.ModAsset.Trail_13_black.Value;
					break;
			}

			TrailLength = 9;
			TrailWidth = 60;
		}
		if (Style == 1)
		{
			Projectile.extraUpdates = 3;
			TrailTexture = Commons.ModAsset.Trail_10.Value;
			TrailTextureBlack = Commons.ModAsset.Trail_10_black.Value;
			TrailLength = 32;
			TrailWidth = 15;
			Projectile.damage = (int)(Projectile.damage * 1.5f);
		}
		base.OnSpawn(source);
	}

	public override void Behaviors()
	{
		if (TimeAfterEntityDestroy >= 0)
		{
			Projectile.velocity *= 0;
			if (Style == 0)
			{
				if (TimeAfterEntityDestroy <= TrailLength - 2)
				{
					Projectile.friendly = false;
				}
			}
			return;
		}
		if (Style == 0)
		{
			Projectile.rotation = 0;
			Projectile.velocity *= 0.995f;
			Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.5f, 0.1f));
			var target = Projectile.FindTargetWithinRange(600);
			if (target is not null)
			{
				Vector2 toTarget = target.Center - Projectile.Center;
				toTarget = toTarget.NormalizeSafe() * 24f;
				Projectile.velocity = Projectile.velocity * 0.92f + toTarget * 0.08f;
			}
			if (Main.rand.NextBool())
			{
				float sqrtSpeed = MathF.Sqrt(Main.rand.NextFloat(1f));
				Vector2 newVelocity = new Vector2(0, sqrtSpeed * 0.3f).RotatedByRandom(MathHelper.TwoPi) + Projectile.velocity;
				var somg = new LanternFlameDust
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi) - Projectile.velocity,
					MaxTime = Main.rand.Next(30, 45),
					Scale = Main.rand.NextFloat(2f, 48f),
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
				};
				Ins.VFXManager.Add(somg);
			}
			if (Timer > 30)
			{
				if (Projectile.wet && !Projectile.lavaWet)
				{
					DestroyEntity();
				}
			}
		}
		if (Style == 1)
		{
		}
		if (Timer > 30)
		{
			Projectile.tileCollide = true;
		}
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
	}

	public override void DestroyEntityEffect()
	{
		if (Style == 0)
		{
			for (int g = 0; g < 12; g++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(17f, 30f)).RotatedByRandom(MathHelper.TwoPi);
				var spark = new HitEffectSpark
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = Main.rand.Next(16, 20),
					DrawColor = new Color(1f, 0.4f, 0, 0),
					LightFlat = 1f,
					SpeedDecay = 0.8f,
					GravityAcc = 0.15f,
					SelfLight = true,
					Scale = Main.rand.NextFloat(16f, 28f),
				};
				Ins.VFXManager.Add(spark);
			}
			for (int g = 0; g < 24; g++)
			{
				float sqrtSpeed = MathF.Sqrt(Main.rand.NextFloat(1f));
				Vector2 newVelocity = new Vector2(0, sqrtSpeed * 12).RotatedByRandom(MathHelper.TwoPi);
				var somg = new LanternFlameDust
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi),
					MaxTime = Main.rand.Next(30, 60),
					Scale = Main.rand.NextFloat(64f, 120f),
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
				};
				Ins.VFXManager.Add(somg);
			}
			SoundStyle sound;
			switch (Projectile.whoAmI % 3)
			{
				case 0:
					sound = new SoundStyle(ModAsset.LanternYoyo_Burst0_Mod);
					break;
				case 1:
					sound = new SoundStyle(ModAsset.LanternYoyo_Burst1_Mod);
					break;
				case 2:
					sound = new SoundStyle(ModAsset.LanternYoyo_Burst2_Mod);
					break;
				default:
					sound = new SoundStyle(ModAsset.LanternYoyo_Burst0_Mod);
					break;
			}

			SoundEngine.PlaySound(sound.WithPitchOffset(-0.5f), Projectile.Center);
		}
		if (Style == 1)
		{
			var star = new HitStarAndWave
			{
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				Rotation = Projectile.oldVelocity.ToRotation(),
				MaxTime = Main.rand.Next(20, 30),
				Scale = Main.rand.NextFloat(0.7f, 0.96f),
			};
			Ins.VFXManager.Add(star);
			for (int g = 0; g < 6; g++)
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(23f, 40f)).RotatedByRandom(MathHelper.TwoPi);
				var spark = new HitEffectSpark
				{
					Velocity = newVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = Main.rand.Next(12, 16),
					DrawColor = new Color(0.8f, 0.8f, 0, 0),
					LightFlat = 0f,
					SpeedDecay = 0.9f,
					GravityAcc = 0.0f,
					SelfLight = false,
					Scale = Main.rand.NextFloat(20f, 30f),
				};
				Ins.VFXManager.Add(spark);
			}
		}
	}

	public override void OnKill(int timeLeft)
	{
		base.OnKill(timeLeft);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void DestroyEntity()
	{
		if (Style == 0)
		{
			Projectile.velocity = Projectile.oldVelocity;
			var oldCenter = Projectile.Center;
			Projectile.width = 120;
			Projectile.height = 120;
			Projectile.Center = oldCenter;
			if (TimeAfterEntityDestroy < 0)
			{
				DestroyEntityEffect();
			}
			TimeAfterEntityDestroy = TrailLength;
			return;
		}
		base.DestroyEntity();
	}

	public override void DrawSelf()
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		Texture2D tex_black = ModAsset.MillionLightStaff_Proj_Shade.Value;
		Rectangle frame = new Rectangle(0, 38 * Style, 38, 38);
		Color drawColor = new Color(1f, 1f, 1f, 0);
		if (Style == 1)
		{
			drawColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		}
		if (Projectile.timeLeft < 60)
		{
			drawColor *= Projectile.timeLeft / 60f;
		}
		if (Style == 0)
		{
			Main.EntitySpriteDraw(tex_black, Projectile.Center - Main.screenPosition, frame, Color.White * 0.7f, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, drawColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor + Projectile.whoAmI / 3.5f;
		if (Style == 0)
		{
			x -= timeValue;
		}
		float y = 1;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 1)
		{
			if (Style == 0)
			{
				Color drawC = Color.Lerp(new Color(1f, 0.6f, 0.1f, 0), new Color(0.7f, 0f, 0.05f, 0), factor);
				if (index < 4)
				{
					drawC = Color.Lerp(drawC, new Color(1f, 1f, 0.6f, 0), 1 - index / 4f);
				}
				return drawC;
			}

			if (Style == 1)
			{
				Color goldenEnv = Lighting.GetColor(worldPos.ToTileCoordinates(), new Color(0.7f, 0.6f, 0.1f, 0));
				goldenEnv.A = 0;
				goldenEnv *= 1.4f;
				return Color.Lerp(goldenEnv, Color.Transparent, factor);
			}
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}
}
