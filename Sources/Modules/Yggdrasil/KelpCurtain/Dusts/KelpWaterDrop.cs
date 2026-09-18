namespace Everglow.Yggdrasil.KelpCurtain.Dusts;

public class KelpWaterDrop : ModDust
{
	public override bool Update(Dust dust)
	{
		if (Collision.IsWorldPointSolid(dust.position + new Vector2(4)))
		{
			dust.active = false;
		}
		return base.Update(dust);
	}
}
