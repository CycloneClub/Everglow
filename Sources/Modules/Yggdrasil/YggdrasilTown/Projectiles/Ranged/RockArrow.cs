using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class RockArrow : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDefaults()
	{
		Projectile.ranged = true;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 6;
		Projectile.timeLeft = 3600;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
	}
	internal int Target = -1;
	internal int TimeTokill = -1;
	public override void OnSpawn(IEntitySource source)
	{
		Projectile.knockBack *= 3;
		Target = (int)Projectile.ai[0];
	}
	public override void AI()
	{
		if (Target == -1)
		{
			Projectile.Kill();
			return;
		}
		Player player = Main.player[Target];
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
			Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - MathHelper.PiOver2;
			Projectile.velocity.Y += 0.05f;
		}
		else
		{
			if (TimeTokill < 10)
			{
				Projectile.damage = 0;
				Projectile.friendly = false;
			}
			Projectile.velocity *= 0f;
		}
	}
	public void GenerateSmog(int Frequency)
	{
		for (int g = 0; g < Frequency / 2 + 1; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 12f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new RockSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(17, 25),
				Scale = Main.rand.NextFloat(40f, 55f),
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
		return false;
	}
	public void AmmoHit()
	{
		TimeTokill = 240;
		Projectile.velocity = Projectile.oldVelocity;
		for (int x = 0; x < 9; x++)
		{
			Dust.NewDust(Projectile.Center - Projectile.velocity * 2 - new Vector2(4), Projectile.width, Projectile.height, ModContent.DustType<SquamousShellStone>(), 0f, 0f, 0, default, 0.7f);
		}
		for (int x = 0; x < 9; x++)
		{
			var dust = Dust.NewDustDirect(Projectile.Center - Projectile.velocity * 2 - new Vector2(4), 0, 0, ModContent.DustType<TrailStoneSmokeDust>(), 0f, 0f, 0, default, 0.7f);
			dust.velocity = new Vector2(0, Main.rand.NextFloat(5f, 13f)).RotatedByRandom(6.283);
			dust.scale = Main.rand.NextFloat(0.45f, 0.6f);
		}
		GenerateSmog(6);
		SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
		Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<RockExplosion_friendly>(), Projectile.damage, 0, Projectile.owner, 6);
	}
	public override bool PreDraw(ref Color lightColor)
	{
		if (TimeTokill > 0)
		{
			return false;
		}

		var TexMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(TexMain, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TexMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}
	public override void PostDraw(Color lightColor)
	{
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawTrail_dark(lightColor);
		DrawTrail(lightColor);
	}
	public void DrawTrail(Color light)
	{
		float drawC = 0.2f;
		float timer = (float)Main.timeForVisualEffects * 0.02f + Projectile.whoAmI / 17f;
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

			float width = 12;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)trueL;
			var color = Color.Lerp(new Color(drawC * light.R / 255f * 0.3f, drawC * light.G / 255f * 0.2f, drawC * light.B / 255f * 0.1f, 0), new Color(0, 0, 0, 0), factor);
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(Projectile.width / 2f) - Main.screenPosition, color, new Vector3(timer, 0, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(Projectile.width / 2f) - Main.screenPosition, color, new Vector3(timer, 1, 0)));
		}
		if (bars.Count > 2)
		{
			Texture2D t = Commons.ModAsset.Trail_4.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
	public void DrawTrail_dark(Color light)
	{
		float drawC = 0.2f;
		float timer = (float)Main.timeForVisualEffects * 0.02f + Projectile.whoAmI / 17f;
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

			float width = 12;
			if (Projectile.timeLeft <= 30)
			{
				width *= Projectile.timeLeft / 30f;
			}

			var normalDir = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
			normalDir = new Vector2(-normalDir.Y, normalDir.X).SafeNormalize(Vector2.Zero);

			var factor = i / (float)trueL;
			var color = Color.Lerp(new Color(drawC * light.R / 255f, drawC * light.G / 255f, drawC * light.B / 255f, drawC), new Color(0, 0, 0, 0), factor);
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * -width + new Vector2(Projectile.width / 2f) - Main.screenPosition, color, new Vector3(timer, 0, 0)));
			bars.Add(new Vertex2D(Projectile.oldPos[i] + normalDir * width + new Vector2(Projectile.width / 2f) - Main.screenPosition, color, new Vector3(timer, 1, 0)));
		}
		if (bars.Count > 2)
		{
			Texture2D t = Commons.ModAsset.Trail_4_black.Value;
			Main.graphics.GraphicsDevice.Textures[0] = t;
			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}
}
