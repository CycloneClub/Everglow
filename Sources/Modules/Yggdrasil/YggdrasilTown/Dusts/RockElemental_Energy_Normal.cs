using Everglow.Yggdrasil.YggdrasilTown.Projectiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class RockElemental_Energy_Normal : ModDust
{
	public override bool Update(Dust dust)
	{
		dust.position += dust.velocity;
		if (!dust.noGravity)
		{
			dust.velocity.Y += 0.15f;
		}
		dust.velocity *= 0.95f;
		dust.scale *= 0.95f;
		if (dust.scale < 0.05f)
		{
			dust.active = false;
		}
		return false;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(0.7f, 0.3f, 1f, 0);
	}
}
