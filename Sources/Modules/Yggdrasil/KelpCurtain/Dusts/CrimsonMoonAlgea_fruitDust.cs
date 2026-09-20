
namespace Everglow.Yggdrasil.KelpCurtain.Dusts;

public class CrimsonMoonAlgea_fruitDust : ModDust
{
	public override bool Update(Dust dust)
	{
		Lighting.AddLight(dust.position, new Vector3(1f, 0.8f, 0.7f) * dust.scale);
		return base.Update(dust);
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(1f, 1f, 1f, 1f);
	}
}
