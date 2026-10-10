using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.SquamousShell;

public class SquamousRockSpike_Falling : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 80;
		Projectile.height = 80;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 6;
		Projectile.timeLeft = 600;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
		Projectile.hostileDamageScaling = Terraria.DataStructures.GameDifficultyData.NoDifficultyPlayerDamageScaling;
	}

	public int Timer;
	internal int TimeTokill = -1;

	public override void OnSpawn(IEntitySource source)
	{
	}

	public override void AI()
	{
		if (TimeTokill >= 0 && TimeTokill <= 2)
		{
			Projectile.Kill();
		}

		if (TimeTokill <= 15 && TimeTokill > 0)
		{
			Projectile.velocity = Projectile.oldVelocity;
		}

		TimeTokill--;
		if (TimeTokill < 0)
		{
			Projectile.velocity.Y += 1.5f;
		}
		else
		{
			if (TimeTokill < 10)
			{
				Projectile.damage = 0;
				Projectile.hostile = false;
			}
			Projectile.velocity *= 0f;
		}
	}

	public void GenerateSmog()
	{
		ShakerManager.AddShaker(Projectile.Bottom, Vector2.One.RotatedByRandom(MathHelper.Pi), 60, 20f, 60, 0.9f, 0.8f, 150);
		for (int g = 0; g < 15; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(12f, 37f)).RotatedByRandom(MathHelper.TwoPi);
			newVelocity.Y = -Math.Abs(newVelocity.Y);
			newVelocity.Y *= Main.rand.NextFloat(0.1f, 1.4f);
			newVelocity.X *= 0.01f;
			var somg = new RockSmog_Cone_FallingSandDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Bottom + new Vector2(newVelocity.X * 600, 8),
				MaxTime = Main.rand.NextFloat(90, Math.Max(newVelocity.Y * 6, 163)),
				Scale = Main.rand.NextFloat(12f, 13f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < 10; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(6f, 33f)).RotatedByRandom(MathHelper.TwoPi);
			newVelocity.Y = -Math.Abs(newVelocity.Y);
			newVelocity.X *= 0.01f;
			var somg = new RockSmog_Cone_FallingSandDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Bottom + new Vector2(newVelocity.X * 600, 8),
				MaxTime = Main.rand.NextFloat(90, Math.Max(newVelocity.Y * 6, 163)),
				Scale = Main.rand.NextFloat(6f, 7f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		AmmoHit();
		Projectile.timeLeft = 10;
		return false;
	}

	public void AmmoHit()
	{
		TimeTokill = 240;
		Projectile.velocity = Projectile.oldVelocity;
		for (int x = 0; x < 8; x++)
		{
			Dust.NewDust(Projectile.Center - Projectile.velocity * 2 - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<SquamousShellStone>(), 0f, 0f, 0, default, 0.7f);
		}
		for (int x = 0; x < 8; x++)
		{
			Dust.NewDust(Projectile.Center - Projectile.velocity * 2 - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<SquamousShellStone_dark>(), 0f, 0f, 0, default, 0.7f);
		}
		for (int x = 0; x <= 30; x++)
		{
			var d0 = Dust.NewDustDirect(Projectile.Bottom + new Vector2((x - 15) * 4 - 4, -4), 0, 0, ModContent.DustType<SquamousShellWingDust>());
			d0.velocity.Y = -(MathF.Cos((x - 15) / 15f * MathHelper.Pi) + 1) * 8;
			d0.velocity.Y *= Main.rand.NextFloat(0.75f, 1.2f);
			d0.noGravity = false;
		}
		GenerateSmog();
		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithVolume(0.4f), Projectile.Center);
		Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<SquamousRockExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 8);
	}

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		if (TimeTokill > 0)
		{
			return false;
		}
		var textureBloom = ModAsset.SquamousRockSpike_Falling_bloom.Value;
		Main.spriteBatch.Draw(textureBloom, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, textureBloom.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		var textureGlow = ModAsset.SquamousRockSpike_Falling_glow.Value;
		float breathValue = 0.5f + 0.5f * MathF.Sin((float)Main.timeForVisualEffects * 0.24f + Projectile.whoAmI);
		Main.spriteBatch.Draw(textureGlow, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, breathValue) * breathValue, Projectile.rotation, textureGlow.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public override void PostDraw(Player player, Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawTrail_dark(lightColor);
		DrawTrail(lightColor);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}

	public void DrawTrail(Color light)
	{
		float dissolveDuration = (3600 - Projectile.timeLeft) / 80f - 0.7f;
		if (Projectile.timeLeft < 3450)
		{
			dissolveDuration = 1;
		}
		float drawC = 0.4f * dissolveDuration;

		var bars = new List<Vertex2D>();
		int trueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				if (i == 1)
				{
					return;
				}

				break;
			}

			trueL = i;
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 80;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)trueL;
			var color = Color.Lerp(new Color(drawC * light.R / 255f * 0.3f, drawC * light.G / 255f * 0.2f, drawC * light.B / 255f * 0.1f, 0), new Color(0, 0, 0, 0), factor);
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(Projectile.width / 2f) - Main.screenPosition, color, new Vector3(1, 0, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(Projectile.width / 2f) - Main.screenPosition, color, new Vector3(1, 1, 0)));
		}
		if (bars.Count > 2)
		{
			Texture2D t = Commons.ModAsset.Trail_4.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public void DrawTrail_dark(Color light)
	{
		float dissolveDuration = (3600 - Projectile.timeLeft) / 80f - 0.7f;
		if (Projectile.timeLeft < 3450)
		{
			dissolveDuration = 1;
		}
		float drawC = 0.4f * dissolveDuration;
		var bars = new List<Vertex2D>();
		int trueL = 0;
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				if (i == 1)
				{
					return;
				}

				break;
			}
			trueL = i;
		}
		for (int i = 1; i < Projectile.oldPos.Length; ++i)
		{
			if (Projectile.oldPos[i] == Vector2.Zero)
			{
				break;
			}

			float width = 80;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)trueL;
			var color = Color.Lerp(new Color(drawC * light.R / 255f, drawC * light.G / 255f, drawC * light.B / 255f, drawC), new Color(0, 0, 0, 0), factor);
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(Projectile.width / 2f) - Main.screenPosition, color, new Vector3(1, 0, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(Projectile.width / 2f) - Main.screenPosition, color, new Vector3(1, 1, 0)));
		}
		if (bars.Count > 2)
		{
			Texture2D t = Commons.ModAsset.Trail_4_black.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
}
