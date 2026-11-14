namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class LampWood_Dust_Fluorescent : ModDust
{
	public override bool Update(Dust dust)
	{
		Lighting.AddLight(dust.position + new Vector2(4), 0.5f * dust.scale, 0.4f * dust.scale, 0);
		return base.Update(dust);
	}
}
