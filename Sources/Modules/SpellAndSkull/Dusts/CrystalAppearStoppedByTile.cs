namespace Everglow.SpellAndSkull.Dusts;

public class CrystalAppearStoppedByTile : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, 0, 16, 16);
		dust.alpha = 0;
		dust.color.R = (byte)(dust.scale * 100f); //用红度存尺寸极值
		dust.color.G = (byte)Main.rand.NextFloat(0f, 255f); //用绿度存相位
		dust.alpha = (byte)Main.rand.NextFloat(0f, 55f); //用透明度存timeleft
	}

	public override bool Update(Dust dust)
	{
		dust.alpha += 4;
		dust.position += dust.velocity;
		dust.velocity += new Vector2(0, 0.015f).RotatedByRandom(MathHelper.Pi * 2d);
		dust.velocity.Y += 0.05f;
		dust.velocity *= 0.95f;
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
