namespace Everglow.Plant.Dusts;

public class CactusJuice : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.frame = new Rectangle(0, 16 * Main.rand.Next(8), 16, 16);
		dust.alpha = 0;
		dust.rotation = dust.scale * 0.3f; // 用旋转角度存尺寸极值
	}

	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.velocity *= 0.995f;
		dust.velocity.Y += 0.1f;
		if (Collision.SolidCollision(dust.position, 8, 8))
		{
			if (dust.scale > 0.3f)
			{
				for (int x = 0; x < 4; x++)
				{
					Vector2 v0 = dust.velocity;
					int T = 0;
					while (Collision.SolidCollision(dust.position + v0, 8, 8))
					{
						T++;
						v0 = v0.RotatedByRandom(6.283);
						if (T > 10)
						{
							v0 = dust.velocity * 0.5f;
							break;
						}
					}
					Dust.NewDust(dust.position + new Vector2(8) * dust.scale + v0 * 5, 0, 0, dust.type, v0.X, v0.Y, 0, default(Color), dust.scale * 0.666f);
				}
				dust.active = false;
			}
			else
			{
				dust.scale *= 0.7f;
			}
		}
		if (dust.scale < 0.05f)
		{
			dust.active = false;
		}

		dust.scale *= 0.995f;
		return false;
	}
	// public override Color? GetAlpha(Dust dust, Color lightColor)
	// {
	//    return new Color?(new Color(255, 255, 255, 0f));
	// }
}
