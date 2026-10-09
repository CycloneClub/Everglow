using Everglow.SpellAndSkull.Projectiles.CrystalStorm;

namespace Everglow.SpellAndSkull.Dusts;

public class CrystalAppearStoppedByTileInAStorm : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, 0, 16, 16);
		dust.color.R = (byte)(dust.scale * 100f); // 用红度存尺寸极值
		dust.color.G = (byte)Main.rand.NextFloat(0f, 255f); // 用绿度存相位
		dust.alpha = (byte)Main.rand.NextFloat(0f, 55f); // 用透明度存timeleft
	}

	public override bool Update(Dust dust)
	{
		Vector2 AimCenter = Main.projectile[dust.dustIndex].Center;
		float Dy = AimCenter.Y - dust.position.Y;
		float xCoefficient = Dy * Dy / 600f - 0.4f * Dy + 50;

		dust.alpha += 3;
		dust.position += dust.velocity;
		dust.color.B = (byte)(dust.color.B * 0.95 + xCoefficient * 0.05); // 蓝度用来存加速度

		if (!Main.projectile[dust.dustIndex].active || Main.projectile[dust.dustIndex].type != ModContent.ProjectileType<Storm>())
		{
			dust.velocity.Y += 0.05f;
			dust.velocity *= 0.95f;
		}
		else
		{
			dust.velocity = dust.velocity * 0.75f + new Vector2((AimCenter + new Vector2(xCoefficient * (float)Math.Sin(Main.timeForVisualEffects * 0.1 + dust.color.G), 0) - dust.position).SafeNormalize(new Vector2(0, 0.05f)).X, -dust.color.A/*颜色透明度用来存上升系数*/ / 700f) * 0.25f / dust.color.B * 500f;
		}

		dust.scale = (float)(Math.Sin(dust.alpha / 25d * Math.PI + dust.color.G) + 1.5f) * dust.color.R * 0.003f;
		if (dust.alpha > 200)
			dust.scale *= (255 - dust.alpha) / 55f;
		Lighting.AddLight(dust.position, 0.0096f * dust.scale / 1.8f, 0.0955f * dust.scale / 1.8f, 0.4758f * dust.scale / 1.8f);
		if (Collision.SolidCollision(dust.position, 8, 8))
		{
			Vector2 v0 = dust.velocity;
			int T = 0;
			while (Collision.SolidCollision(dust.position + v0, 8, 8))
			{
				T++;
				v0 = v0.RotatedByRandom(6.283);
				if (T > 10)
				{
					v0 = dust.velocity * 0.9f;
					break;
				}
			}
			dust.velocity = v0;
		}
		if (dust.alpha > 254)
			dust.active = false;

		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color?(new Color(255, 255, 255, 0f));
	}
}
