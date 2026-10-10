namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class WhisperingGhostGasSmalling : ModDust
{
	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		return lightColor;
	}

	public override void OnSpawn(Dust dust)
	{
		base.OnSpawn(dust);
	}

	public override bool Update(Dust dust)
	{
		dust.scale *= 0.98f;
		dust.rotation += 0.4f;
		dust.position += dust.velocity;
		dust.velocity *= 0.96f;
		if (dust.scale < 0.4)
		{
			dust.active = false;
		}
		return false;
	}
}
