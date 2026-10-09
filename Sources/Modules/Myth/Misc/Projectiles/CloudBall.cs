using Terraria.Audio;
using Terraria.Localization;

namespace Everglow.Myth.Misc.Projectiles;

public class CloudBall : ModProjectile
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("CloudBall");
		ProjectileID.Sets.TrailingMode[Type] = 2;
		ProjectileID.Sets.TrailCacheLength[Type] = 10;
	}

	public override void SetDefaults()
	{
		Projectile.width = 14;
		Projectile.height = 14;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 3600;
		Projectile.hostile = false;
		Projectile.alpha = 255;
		Projectile.extraUpdates = 3;
	}

	/// <summary>
	/// 粒子最后生成后额外距离
	/// </summary>
	private float DistanceCompletion
	{
		get { return Projectile.ai[0]; }
		set { Projectile.ai[0] = value; }
	}

	/// <summary>
	/// 粒子释放间距，也就是说不管速度多快，都是每隔8帧释放一次粒子
	/// </summary>
	private static int DustSpacing = 8;

	public override void AI()
	{
		if (Projectile.timeLeft > 3597)
		{
			return;
		}

		Projectile.alpha = 100;
		Vector2 normalize = Projectile.velocity.SafeNormalize(Vector2.Zero);
		float speed = MathF.Round(Projectile.velocity.Length(), 0);
		if (DistanceCompletion >= speed)
			DistanceCompletion -= speed;
		else
		{
			for (float i = DistanceCompletion; i < speed; i += DustSpacing)
			{
				// Main.NewText($"time:{Main.time} speed:{speed} i:{i}");
				DistanceCompletion = DustSpacing - speed + i;
				Vector2 pos = Projectile.Center + new Vector2(-4, -4);
				pos += normalize * i;

				int r = Dust.NewDust(pos, 0, 0, DustID.Cloud, 0, 0, 0, default, 2f);
				Main.dust[r].noGravity = true;
				Main.dust[r].velocity *= 0.3f;
				// 这是另一个粒子效果
				/*for (float j = 0; j < MathHelper.TwoPi; j += MathHelper.PiOver4)
                    {
                        Vector2 pos2 = pos + j.ToRotationVector2() * 10f;
                        Vector2 vel = (pos - pos2).SafeNormalize(Vector2.Zero);
                        int r = Dust.NewDust(pos2, 0, 0, DustID.Cloud, 0, 0, 0, default, 1.5f);
                        Main.dust[r].noGravity = true;
                        Main.dust[r].velocity = vel * 2.5f;
                    }*/
			}
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.velocity = oldVelocity;
		Projectile.Kill();
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		// 这是原来的粒子效果
		/*for (int i = 0; i < 30; i++)
            {
                Vector2 pos = new(Projectile.position.X, Projectile.position.Y);
                Vector2 vel = new Vector2(0, Main.rand.NextFloat(3.3f, 3.7f)).RotatedBy(i / 15d * Math.PI);
                int num = Dust.NewDust(pos, Projectile.width, Projectile.height, DustID.Cloud, 0f, 0f, 100, default, 2f);
                Main.dust[num].velocity = vel;
                Main.dust[num].noGravity = true;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }*/

		SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
		for (int i = 0; i < 30; i++)
		{
			Vector2 pos = Projectile.position;
			Vector2 vel = (Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.Next(1, 20)).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
			int num = Dust.NewDust(pos, Projectile.width, Projectile.height, DustID.Cloud, 0f, 0f, 0, default, 2f);
			Main.dust[num].velocity = vel;
			Main.dust[num].noGravity = true;
		}
	}
}
