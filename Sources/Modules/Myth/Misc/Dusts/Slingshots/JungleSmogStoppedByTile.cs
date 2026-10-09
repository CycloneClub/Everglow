namespace Everglow.Myth.Misc.Dusts.Slingshots;

public class JungleSmogStoppedByTile : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.noGravity = true;
		dust.noLight = false;
		dust.scale *= 1f;
		dust.alpha = 0;
	}

	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.rotation += 0.1f;
		dust.velocity *= 0.95f;
		dust.alpha++;
		Lighting.AddLight(dust.position, (float)((255 - dust.alpha) * 0.0005f), (float)((255 - dust.alpha) * 0.0045f), 0);
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
					v0 *= -1;
					break;
				}
			}
			dust.velocity = v0;
		}
		if (dust.alpha > 254)
			dust.active = false;
		// 低损耗挂毒
		int LuckTarget = Main.rand.Next(200);
		NPC target = Main.npc[LuckTarget];
		if (target.active)
		{
			if (!target.dontTakeDamage)
			{
				if (!target.friendly)
				{
					if (!target.buffImmune[BuffID.Poisoned])
					{
						if ((target.Center - dust.position).Length() < 60)
							target.AddBuff(BuffID.Poisoned, 180);
					}
				}
			}
		}
		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		float k = (255 - dust.alpha) / 255f;
		if (dust.scale > 0.6f)
			return new Color?(new Color(0.3f * k * k, 0.9f * k, 0, 0f));
		else
		{
			return new Color?(new Color(0.3f * k * k, 0.9f * k, 0, 0));
		}
	}
}
