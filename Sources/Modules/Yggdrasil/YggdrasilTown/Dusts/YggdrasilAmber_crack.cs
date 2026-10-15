namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class YggdrasilAmber_crack : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.velocity *= 0;
		dust.color.A = (byte)Main.rand.Next(255); // 透明度存角速度
		dust.color.R = (byte)Main.rand.Next(120, 255); // 红度存黑化率
		dust.frame = new Rectangle(0, Main.rand.Next(6) * 9, 8, 9);
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		if ((int)Main.time % 50 == TileUtils.GetFixedRandomNumber_SingleSeed(dust.GetHashCode(), 50))
		{
			dust.alpha = 1;
			return new Color?(new Color(1f, 1f, 1f, 0));
		}
		else
		{
			dust.alpha = 150;
			return new Color?(new Color(dust.scale / 3.7f * lightColor.R / 255f, dust.scale / 3.7f * lightColor.G / 255f, dust.scale / 3.7f * lightColor.B / 255f, 1 * dust.color.R / 155f - dust.scale));
		}
	}

	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.rotation += (dust.color.A - 127.5f) / 255f;
		dust.scale *= 0.96f;
		dust.velocity *= 0.95f;
		if (dust.scale < 0.15f)
		{
			dust.active = false;
		}

		return false;
	}
}
