using Everglow.EternalResolve.Projectiles;

namespace Everglow.EternalResolve.Items.Weapons.StabbingSwords.Dusts
{
	/// <summary>
	/// 用color.R和color.G存归属于哪一个Proj
	/// </summary>
	public class RottenSmog : ModDust
	{
		public override bool Update(Dust dust)
		{
			if (dust.color.G * 255 + dust.color.R >= Main.projectile.Length)
			{
				dust.active = false;
			}
			Projectile proj = Main.projectile[dust.color.G * 255 + dust.color.R];
			if (!proj.active || proj == null || proj.type != ModContent.ProjectileType<RottenGoldBayonet_Mark>())
			{
				dust.scale *= 0.1f;
			}
			else if (proj.timeLeft > 54f)
			{
				Vector2 acceleration = proj.Center - dust.position;
				Vector2 normalizeAcceleration = Utils.SafeNormalize(acceleration, Vector2.zeroVector);
				float value3 = 60f;
				dust.velocity += normalizeAcceleration / acceleration.Length() * MathF.Pow((120 + value3 - proj.timeLeft) / value3, 1.0f) * 100f;
				// dust.velocity += (acceleration.Length() - 20) / 400f * normalizeAcceleration;
				dust.scale += 0.02f;
			}
			else if (proj.timeLeft == 54f)
			{
				Vector2 acceleration = proj.Center - dust.position;
				Vector2 normalizeAcceleration = Utils.SafeNormalize(acceleration, Vector2.zeroVector);
				dust.velocity = -normalizeAcceleration * Main.rand.NextFloat(0.1f, 8f);
			}
			else
			{
				dust.velocity *= 0.95f;
				dust.scale *= 0.95f;
			}
			if (dust.scale < 0.02f)
			{
				dust.active = false;
			}

			dust.position += dust.velocity;
			dust.rotation += 0.02f;
			return false;
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			Color c0 = lightColor;
			c0.A = dust.color.A;
			return c0;
		}

		public override void OnSpawn(Dust dust)
		{
			dust.color.A = 150;
			dust.frame = new Rectangle(0, Main.rand.Next(3) * 9, 8, 9);
			base.OnSpawn(dust);
		}
	}
}
