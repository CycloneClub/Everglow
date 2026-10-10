namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class JellyBallSpark : ModDust
{
	public override bool Update(Dust dust)
	{
		Lighting.AddLight(dust.position + new Vector2(4), 0, 0.8f * dust.scale, 0.9f * dust.scale);
		return base.Update(dust);
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return new Color(1f, 1f, 1f, 0);
	}
}
