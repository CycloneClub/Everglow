using Everglow.Commons.Templates.Weapons;
using Everglow.Myth.TheFirefly.VFXs;
using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class MothMiddleBullet : TrailingProjectile
{
	public override void SetCustomDefaults()
	{
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.timeLeft = 1600;
		Projectile.tileCollide = true;
		TrailColor = new Color(0.1f, 0.4f, 0.9f, 0);
		TrailTexture = Commons.ModAsset.Trail_6.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_0_black.Value;
	}

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
	}

	public override void Behaviors()
	{
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		if (Projectile.timeLeft > 300 && Projectile.timeLeft < 1500)
		{
			Player player = Main.player[Player.FindClosest(Projectile.Center, 0, 0)];
			Vector2 toPlayer = player.Center - Projectile.Center - Projectile.velocity;
			if (toPlayer.Length() > 200)
			{
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(toPlayer) * 8f, 0.005f + 0.0025f * MathF.Sin((float)Main.time * 0.015f + Projectile.whoAmI) + Projectile.ai[0]);
			}
		}
		if (Projectile.timeLeft < 200)
		{
			DestroyEntity();
		}
		GenerateFire(1);
		GenerateSpark(2);
	}

	public void GenerateFire(int frequency)
	{
		float mulVelocity = Projectile.ai[0] / 5f;
		for (int g = 0; g < frequency; g++)
		{
			float sqrtRand = MathF.Pow(Main.rand.NextFloat(1), 0.4f);
			Vector2 newVelocity = new Vector2(0, mulVelocity * sqrtRand * 0.6f).RotatedByRandom(MathHelper.TwoPi);
			var fire = new MothBlueFireDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity,
				MaxTime = Main.rand.Next(19, 35),
				Scale = Main.rand.NextFloat(0.2f, 0.5f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, 0 },
			};
			Ins.VFXManager.Add(fire);
		}
	}

	public void GenerateSpark(int frequency)
	{
		float mulVelocity = Projectile.ai[0] / 5f;
		for (int g = 0; g < frequency; g++)
		{
			float sqrtRand = MathF.Pow(Main.rand.NextFloat(1), 0.4f);
			Vector2 newVelocity = new Vector2(0, mulVelocity * sqrtRand * 0.9f).RotatedByRandom(MathHelper.TwoPi);
			var smog = new MothShimmerScaleDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				Coord = new Vector2(Main.rand.NextFloat(1f), Main.rand.NextFloat(1f)),
				MaxTime = Main.rand.Next(10, 95),
				Scale = Main.rand.NextFloat(0.4f, 2.4f),
				Rotation = Main.rand.NextFloat(6.283f),
				Rotation2 = Main.rand.NextFloat(6.283f),
				Omega = Main.rand.NextFloat(-30f, 30f),
				Phi = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(-0.005f, 0.005f), 0, 0 },
			};
			Ins.VFXManager.Add(smog);
		}
	}

	public override void DrawTrail()
	{
		base.DrawTrail();
	}

	public override void DrawSelf()
	{
		var texDark = ModAsset.MothMiddleBullet_dark.Value;
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		var texStar = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(texDark, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texDark.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

		Vector2 addVel = Vector2.Normalize(Projectile.velocity) * 10;
		Main.spriteBatch.Draw(texStar, Projectile.Center - Main.screenPosition + addVel, null, TrailColor, 0, texStar.Size() / 2f, 0.5f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(texStar, Projectile.Center - Main.screenPosition + addVel, null, TrailColor, MathHelper.PiOver2, texStar.Size() / 2f, new Vector2(0.5f, 1f), SpriteEffects.None, 0);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0) => base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor - timeValue * 0.6f;
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

	public override void DestroyEntityEffect()
	{
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<MothBulletExplosion>(), 50, 3, Projectile.owner, 10f);
	}
}
