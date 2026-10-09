namespace Everglow.Myth.LanternMoon.Dusts;

public class Flame : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.velocity *= 0.1f;
		dust.noGravity = true;
		dust.noLight = false;
		dust.scale *= 1f;
		dust.alpha = 0;
	}
	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		dust.rotation += 0.1f;
		dust.scale *= 0.95f;
		dust.velocity *= 0.95f;
		float scale = dust.scale;
		Lighting.AddLight(dust.position, dust.color.R * 0.0005f, dust.color.G * 0.0005f, dust.color.B * 0.0005f);
		if (dust.scale < 0.25f)
		{
			dust.active = false;
		}
		// for(int i = 0; i < 200;i++)
		// {
		//    if((Main.npc[i].Center - dust.position).Length() < 10 && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly)
		//    {
		//        Main.npc[i].StrikeNPC((int)(20000 * Main.rand.NextFloat(0.8f, 1.2f)) / 3 * 2, 0, (int)(Main.npc[i].velocity.X / Math.Abs(Main.npc[i].velocity.X)), false, false, false);
		//    }
		// }
		return false;
	}
	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		if (dust.scale > 1.5f)
		{
			return new Color?(new Color(1, 0.5f, 0.1f, 0.1f));
		}
		else
		{
			return new Color?(new Color(dust.scale / 1.5f, dust.scale * dust.scale / 4.5f, dust.scale * dust.scale / 22.5f, (1.5f - dust.scale) / 1.5f * 0.9f + 0.1f));
		}
	}
}
