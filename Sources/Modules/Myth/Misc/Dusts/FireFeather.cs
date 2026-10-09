using static Terraria.ModLoader.PlayerDrawLayer;

namespace Everglow.Myth.Misc.Dusts;

public class FireFeather : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.noLight = false;
		dust.scale *= 1f;
		dust.alpha = 0;
		dust.color.R = (byte)Main.rand.Next(255);
		dust.color.A = (byte)Main.rand.Next(130, 205);
		dust.rotation = Main.rand.NextFloat(6.283f);
	}
	public override bool Update(Dust dust)
	{
		var tile = Main.tile[(int)(dust.position.X / 16), (int)(dust.position.Y / 16)];
		bool wet = dust.position.Y % 1 < tile.LiquidAmount / 256f;
		dust.position += dust.velocity;
		dust.scale *= 0.97f;
		dust.velocity *= MathF.Pow(0.97f, dust.velocity.Length());
		if (!wet)
		{
			Lighting.AddLight(dust.position, (dust.scale - 0.5f) * 0.85f, (dust.scale - 0.5f) * 0.25f, 0);
			dust.rotation += MathF.Sin((float)(Main.time * 0.3 + dust.color.R)) * 0.7f;
		}
		else
		{
			dust.rotation *= 0.96f;
			dust.velocity.Y -= 0.03f;

		}
		if (dust.position.X <= 320 || dust.position.X >= Main.maxTilesX * 16 - 320)
		{
			dust.active = false;
		}
		if (dust.position.Y <= 320 || dust.position.Y >= Main.maxTilesY * 16 - 320)
		{
			dust.active = false;
		}
		if (Collision.SolidCollision(dust.position, 8, 8))
		{
			Vector2 v0 = dust.velocity;
			int t = 0;
			while (Collision.SolidCollision(dust.position + v0, 8, 8))
			{
				t++;
				v0 = v0.RotatedByRandom(6.283);
				if (t > 10)
				{
					v0 = dust.velocity * 0.9f;
					break;
				}
			}
			dust.velocity = v0;
			dust.scale *= 0.9f;
		}
		else
		{
			if (!wet)
			{
				dust.velocity += new Vector2(Main.windSpeedCurrent * 0.25f, 0.02f * dust.scale * dust.scale);
			}
		}
		if (dust.scale < 0.5f)
		{
			dust.active = false;
		}

		return false;
	}
	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		var tile = Main.tile[(int)(dust.position.X / 16), (int)(dust.position.Y / 16)];
		bool wet = dust.position.Y % 1 < tile.LiquidAmount / 256f;
		float value = 1f;
		if (dust.scale < 1.5f)
		{
			value = dust.scale - 0.5f;
		}
		if (wet)
		{
			value = 0.05f;
		}
		return new Color(value, value, value * 1.4f, dust.color.A / 255f);
	}
}
