using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;

public class BloodSpell : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.EnemyProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 24;
		Projectile.height = 24;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 1;
		Projectile.scale = 0;
		Projectile.timeLeft = 3600;
		Projectile.tileCollide = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.timeLeft = Main.rand.Next(3600, 3720);
	}

	public Vector2 BreakingPos = Vector2.zeroVector;

	public override void AI()
	{
		Player player = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		if (Projectile.timeLeft < 3450)
		{
			if (Projectile.velocity.Length() < 1f)
			{
				if (Projectile.timeLeft > 3000)
				{
					float speed = 8;
					if (Projectile.timeLeft > 3400)
					{
						speed = 16;
					}
					Projectile.velocity = Vector2.Normalize(player.Center - Projectile.Center).RotatedByRandom(0.5f) * speed;
					if (speed > 8)
					{
						BreakingPos = Projectile.Center;
						ShakeEffectLarge();
					}
					else
					{
						WaveEffect();
					}
					Projectile.scale = 1f;
				}
			}
			else
			{
				// Projectile.velocity *= 0.96f;
				// Projectile.scale *= 0.96f;
				if (Projectile.timeLeft % 120 > 60 && Projectile.timeLeft > 3000)
				{
					Projectile.velocity = (player.Center - Projectile.Center - Projectile.velocity).SafeNormalize(Vector2.zeroVector) * 6f * 0.05f + Projectile.velocity * 0.95f;
				}
			}
			if (Main.rand.NextFloat(Projectile.velocity.Length()) > 0.5f)
			{
				GenerateDust(1);
			}
			if (Main.rand.NextFloat(Projectile.velocity.Length()) > 1.5f)
			{
				GenerateDust(1);
			}
			if (Main.rand.NextFloat(Projectile.velocity.Length()) > 3.5f)
			{
				GenerateDust(1);
			}
		}
		else
		{
			if (Projectile.timeLeft < 3550)
			{
				Projectile.velocity.Y = -0.5f;
			}
			else
			{
				Projectile.velocity.Y = 0;
			}
			Projectile.velocity.X = 0;
			Projectile.scale += 0.01f;
		}
		Projectile.rotation += 0.3f;
		Lighting.AddLight(Projectile.Center, Projectile.scale * 1.6f, 0, 0);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		if (Projectile.timeLeft > 3360 && Projectile.velocity.Length() > 12f)
		{
			return (targetHitbox.Center() - BreakingPos).Length() < 200f;
		}
		return base.Colliding(projHitbox, targetHitbox);
	}

	public void ShakeEffectLarge()
	{
		for (int g = 0; g < 550; g++)
		{
			float speed = MathF.Sin(g / 55f * MathHelper.TwoPi) * 0.13f + 1;
			speed *= 9;
			Vector2 velocity = new Vector2(0, speed).RotatedBy(g / 550f * MathHelper.TwoPi);
			var somg = new BloodFlame
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = 500,
				Scale = 5,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f), g / 55f * MathHelper.TwoPi + MathHelper.PiOver2, MathF.Sin(g / 55f * MathHelper.TwoPi) * 0.13f },
			};
			Ins.VFXManager.Add(somg);

			speed = MathF.Sin(g / 55f * MathHelper.TwoPi) * 0.13f + 1;
			speed *= 13;
			velocity = new Vector2(0, speed * Main.rand.NextFloat(0.995f, 1.003f)).RotatedBy(g / 550f * MathHelper.TwoPi);
			var somg2 = new BloodFlame_dark
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = 500,
				Scale = 3,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f), g / 25f * MathHelper.TwoPi + MathHelper.PiOver2, MathF.Sin(g / 25f * MathHelper.TwoPi) * 0.08f },
			};
			Ins.VFXManager.Add(somg2);
		}
	}

	public void WaveEffect()
	{
		var wave = new BloodSpellShake
		{
			Active = true,
			Visible = true,
			Position = Projectile.Center,
			MaxTime = Main.rand.Next(57, 66),
			Scale = 0.25f,
			Rotation = 0,
			ai = new float[] { 0, 0, 0 },
		};
		Ins.VFXManager.Add(wave);

		var wave2 = new BloodSpellShake
		{
			Active = true,
			Visible = true,
			Position = Projectile.Center,
			MaxTime = Main.rand.Next(57, 66),
			Scale = 0.35f,
			Rotation = MathHelper.Pi / 12f,
			ai = new float[] { MathHelper.Pi, 0, 0 },
		};
		Ins.VFXManager.Add(wave2);

		for (int g = 0; g < 15; g++)
		{
			Vector2 velocity = new Vector2(0, Main.rand.NextFloat(0.7f, 1.93f)).RotatedBy(g / 15f * MathHelper.TwoPi);

			velocity = velocity.RotatedBy(Projectile.rotation + Projectile.whoAmI);

			var somg = new BloodFlame
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = 500,
				Scale = 2,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f), 0, 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public void GenerateDust(int frequency)
	{
		for (int g = 0; g < frequency; g++)
		{
			float value = Main.rand.NextFloat(0, 1f);
			Vector2 lightingCrack = Projectile.velocity.SafeNormalize(Vector2.zeroVector).RotatedBy(MathHelper.PiOver2);
			Vector2 pos = Projectile.Center - Projectile.velocity * value;
			lightingCrack *= 1 * MathF.Sin(pos.Length() * 0.1f + Projectile.whoAmI);
			lightingCrack += lightingCrack.RotatedBy(MathHelper.PiOver2 * MathF.Sin((float)Main.time * 0.1f + Projectile.whoAmI) * 0.3f) * 0.5f * MathF.Sin(Projectile.velocity.Length() * 3f);
			var somg = new BloodFlame
			{
				Velocity = lightingCrack,
				Active = true,
				Visible = true,
				Position = pos,
				MaxTime = Main.rand.Next(117, 125),
				Scale = 1,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f), pos.Length() * 0.14f + Projectile.whoAmI * 0.4f, 0.1f },
			};
			Ins.VFXManager.Add(somg);

			lightingCrack = Projectile.velocity.SafeNormalize(Vector2.zeroVector).RotatedBy(MathHelper.PiOver2);
			lightingCrack *= 1 * MathF.Sin(pos.Length() * 0.22f + Projectile.whoAmI);
			lightingCrack += lightingCrack.RotatedBy(MathHelper.PiOver2 * MathF.Sin((float)Main.time * 0.07f + Projectile.whoAmI) * 0.2f) * 0.6f * MathF.Sin(Projectile.velocity.Length() * 2f);
			var somg2 = new BloodFlame_dark
			{
				Velocity = lightingCrack,
				Active = true,
				Visible = true,
				Position = pos,
				MaxTime = Main.rand.Next(117, 125),
				Scale = 1,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f), pos.Length() * 0.21f + Projectile.whoAmI * 0.71f, 0.07f },
			};
			Ins.VFXManager.Add(somg2);
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		return true;
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D fbm = Commons.ModAsset.FBM.Value;
		Texture2D shade = Commons.ModAsset.Point_black.Value;
		Main.spriteBatch.Draw(shade, Projectile.Center - Main.screenPosition - Projectile.velocity, null, new Color(1f, 1f, 1f, 1f), Projectile.rotation, shade.Size() / 2f, Projectile.scale * 0.25f + 0.25f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(fbm, Projectile.Center - Main.screenPosition - Projectile.velocity, null, new Color(1f, 0f, 0f, 0f), Projectile.rotation, fbm.Size() / 2f, Projectile.scale * 0.5f + 0.5f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(fbm, Projectile.Center - Main.screenPosition - Projectile.velocity, null, new Color(1f, 0f, 0f, 0f), Projectile.rotation + (float)Main.time * 0.02f, fbm.Size() / 2f, Projectile.scale * 0.75f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(fbm, Projectile.Center - Main.screenPosition - Projectile.velocity, null, new Color(1f, 0f, 0f, 0f), Projectile.rotation + (float)Main.time * 0.06f, fbm.Size() / 2f, Projectile.scale * 0.35f, SpriteEffects.None, 0);

		var TexMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(TexMain, Projectile.Center - Main.screenPosition - Projectile.velocity, null, new Color(1f, 0f, 0f, 0.5f), Projectile.rotation + (float)Main.time * 0.2f, TexMain.Size() / 2f, Projectile.scale * 0.2f + 0.3f, SpriteEffects.None, 0);

		Texture2D starDark = Commons.ModAsset.StarSlash_black.Value;
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(starDark, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f, MathHelper.PiOver2, starDark.Size() / 2f, new Vector2(0.5f, Projectile.scale * 1.35f + 0.15f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(starDark, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f, 0, starDark.Size() / 2f, new Vector2(0.5f, Projectile.scale * 0.5f + 0.15f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(1f, 0.15f, 0.05f, 0f), MathHelper.PiOver2, star.Size() / 2f, new Vector2(0.5f, Projectile.scale * 1.35f + 0.15f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(1f, 0.15f, 0.05f, 0f), 0, star.Size() / 2f, new Vector2(0.5f, Projectile.scale * 0.5f + 0.15f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(0.5f, 0.2f, 0.2f, 0f), MathHelper.PiOver4, star.Size() / 2f, new Vector2(0.5f, Projectile.scale * 0.3f + 0.05f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(0.5f, 0.2f, 0.2f, 0f), -MathHelper.PiOver4, star.Size() / 2f, new Vector2(0.5f, Projectile.scale * 0.3f + 0.05f), SpriteEffects.None, 0);
		return false;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		Projectile.Kill();
	}

	public override void OnKill(int timeLeft)
	{
		float size = 0.5f;
		for (int g = 0; g < 200; g++)
		{
			Vector2 velocity = new Vector2(0, 8f).RotatedBy(g / 200f * MathHelper.TwoPi);
			if (g >= 150)
			{
				velocity = new Vector2(0, 6f).RotatedBy((g - 140f) / 90f * MathHelper.TwoPi) + new Vector2(3, 3);
			}
			velocity = velocity.RotatedBy(Projectile.rotation + Projectile.whoAmI);
			velocity *= size;
			var somg = new BloodFlame_dark
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = 300,
				Scale = 1,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f), 0, 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		size = 0.2f;
		for (int g = 0; g < 40; g++)
		{
			Vector2 velocity = new Vector2(0, 8f).RotatedBy(g / 40f * MathHelper.TwoPi);
			if (g >= 30)
			{
				velocity = new Vector2(0, 6f).RotatedBy((g - 28f) / 18f * MathHelper.TwoPi) + new Vector2(3, 3);
			}
			velocity = velocity.RotatedBy(Projectile.rotation + Projectile.whoAmI + MathHelper.Pi);
			velocity *= size;
			var somg = new BloodFlame
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = 300,
				Scale = 1,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f), 0, 0 },
			};
			Ins.VFXManager.Add(somg);
		}

		for (int g = 0; g < 120; g++)
		{
			Vector2 velocity = new Vector2(0, Main.rand.NextFloat(4.0f, 10.93f)).RotatedBy(g / 40f * MathHelper.TwoPi);
			var somg = new BloodFlame_trail
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = Projectile.oldPosition + Projectile.Size * 0.5f - Projectile.velocity + velocity,
				MaxTime = 120,
				Scale = 2,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f), 0, 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}
}
