using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.SquamousShell;

public class SquamousRollingStone_fly : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public int StartDirection = 0;
	public int StopTimer = 0;

	public override string Texture => ModAsset.SquamousRollingStone_Mod;

	public override void OnSpawn(IEntitySource source)
	{
		StartDirection = Math.Sign(Projectile.ai[0]);
		for (int t = 0; t < 100; t++)
		{
			if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
			{
				Projectile.position.Y -= 1;
			}
			else
			{
				Projectile.position.Y -= 5;
				break;
			}
		}
	}

	public override void SetDefaults()
	{
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.width = 100;
		Projectile.height = 100;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 1500;
		Projectile.aiStyle = -1;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public override void AI()
	{
		if (Projectile.scale < 0.5)
		{
			return;
		}
		if (MathF.Abs(Projectile.velocity.X) <= 0.1f && MathF.Abs(Projectile.velocity.Y) <= 1f)
		{
			StopTimer++;
			if (StopTimer > 60)
			{
				if (Projectile.timeLeft > 60)
				{
					Projectile.timeLeft = 60;
				}
			}
		}
		else
		{
			StopTimer = 0;
		}
		if (Projectile.timeLeft == 59)
		{
			SoundEngine.PlaySound(new SoundStyle(ModAsset.SquamousShell_RockExplosion_Mod), Projectile.Center);
		}
		if (Projectile.timeLeft == 30)
		{
			var explosion = new RollingRockExplosion
			{
				Velocity = Vector2.zeroVector,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = 240f,
				Scale = Main.rand.NextFloat(940f, 1000f) * Projectile.scale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
			};
			Ins.VFXManager.Add(explosion);
		}
		if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height + 10))
		{
			Projectile.velocity.X *= 0.9f;
		}
		Projectile.velocity.Y += 0.2f;
		Projectile.rotation += Projectile.velocity.X * 0.01f / MathF.Pow(Projectile.scale + 1f, 1.5f);
	}

	public void GenerateSmogAtBottom(int Frequency)
	{
		for (int g = 0; g < Frequency / 2 + 1; g++)
		{
			Vector2 newVelocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(0f, 1f))).RotatedByRandom(MathHelper.TwoPi);
			var somg = new RockSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Bottom + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(17, 45),
				Scale = Main.rand.NextFloat(10f, 25f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Projectile.velocity.Length() * Projectile.scale > 3)
		{
			ShakerManager.AddShaker(Projectile.Center, new Vector2(0, -1), 30 * MathF.Pow(Projectile.scale / 1.2f, 2f), 20f, 120, 0.9f, 0.8f, 150);
		}
		if (Projectile.velocity.X * Projectile.oldVelocity.X < 0)
		{
			Projectile.velocity.X = 0;
		}
		return false;
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		var textureShade = ModAsset.SquamousRollingStone_Shade.Value;
		var textureBloom = ModAsset.SquamousRollingStone_Bloom.Value;
		Main.spriteBatch.Draw(textureShade, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, textureShade.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(textureBloom, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, textureBloom.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

		Texture2D drawTex = ModAsset.SquamousRollingStone.Value;
		Texture2D drawGlow = ModAsset.SquamousRollingStone_glow.Value;
		Main.spriteBatch.Draw(drawTex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, drawTex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		if (Projectile.timeLeft <= 60)
		{
			float glowValue = Projectile.timeLeft / 60f;
			glowValue = (MathF.Sin(1 / (glowValue + 0.01f)) + 0.5f) * (1 - glowValue);
			Main.spriteBatch.Draw(drawGlow, Projectile.Center - Main.screenPosition, null, new Color(glowValue, glowValue, glowValue, 0), Projectile.rotation, drawTex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		for (int x = 0; x < 16; x++)
		{
			Dust.NewDust(Projectile.Center - Projectile.velocity * 2 - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<SquamousShellStone>(), 0f, 0f, 0, default, 0.7f);
		}
		for (int x = 0; x < 40; x++)
		{
			var d0 = Dust.NewDustDirect(Projectile.Bottom - new Vector2(4, -4), 0, 0, ModContent.DustType<SquamousShellWingDust>());
			d0.velocity = new Vector2(0, Main.rand.NextFloat(12, 30)).RotatedByRandom(MathHelper.TwoPi) * Projectile.scale;
			if (d0.velocity.Y > 5f)
			{
				d0.velocity.Y *= -1;
			}
			d0.noGravity = false;
			d0.scale *= Main.rand.NextFloat(1.3f);
		}
		GenerateSmog(8);
		int scaleDamage = 2;
		if (Main.expertMode)
		{
			scaleDamage = 3;
		}
		if (Main.masterMode)
		{
			scaleDamage = 3;
		}
		Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<SquamousRockExplosion>(), Projectile.damage * scaleDamage, Projectile.knockBack, Projectile.owner, 42 * Projectile.scale);
		ShakerManager.AddShaker(Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.Pi), 120 * Projectile.scale, 20f, 120, 0.9f, 0.8f, 150);
	}

	public void GenerateSmog(int Frequency)
	{
		for (int g = 0; g < Frequency / 2 + 1; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi) * Projectile.scale;
			var somg = new RockSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(37, 45),
				Scale = Main.rand.NextFloat(40f, 55f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < Frequency * 10; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(25f, 46f)).RotatedByRandom(MathHelper.TwoPi) * Projectile.scale;
			var somg = new RockSmog_ConeDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(20, 62),
				Scale = Main.rand.NextFloat(0.6f, 25f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < Frequency * 20 * Projectile.scale; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1.0f, 34f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new Spark_MoonBladeDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(70, 125) * (Projectile.scale + 1f) / 2f,
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(9f, 47.0f)) * Projectile.scale,
				Rotation = Main.rand.NextFloat(6.283f),
				noGravity = true,
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}
}
