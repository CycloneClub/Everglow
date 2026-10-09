using Everglow.Commons.Vertex;
using Terraria.DataStructures;

namespace Everglow.MEAC.NonTrueMeleeProj;

public class GoldShieldScale_dust : ModDust
{
	public override void OnSpawn(Dust dust)
	{
		dust.frame = new Rectangle(0, 0, 14, 15);
		base.OnSpawn(dust);
	}

	public override bool Update(Dust dust)
	{
		dust.scale *= 0.92f;
		dust.position += dust.velocity;
		dust.velocity *= 0.96f;
		dust.rotation += dust.velocity.X * 0.02f;
		if (dust.scale < 0.1f)
		{
			dust.active = false;
		}
		return false;
	}
}
