namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class EucaLeafy_EmitGasDust : ModDust
{
	public override bool Update(Dust dust)
	{
		Lighting.AddLight(dust.position, new Vector3(0.15f, 0.4f, 0.9f) * dust.scale);
		return true;
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(1f, 1f, 1f, 0.5f);
	}
}
