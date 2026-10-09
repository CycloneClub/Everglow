namespace Everglow.Yggdrasil.YggdrasilTown.Dusts;

public class WhisperingGhostGas : ModDust
{
	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
	}

	public override Color? GetAlpha(Dust dust, Color lightColor)
	{
		float value = 255 - dust.alpha;
		value /= 255f;
		return lightColor * value;
	}

	public override void OnSpawn(Dust dust)
	{
		base.OnSpawn(dust);
	}

	public override bool Update(Dust dust)
	{
		dust.rotation += 0.4f;
		dust.alpha += 3;
		if (dust.alpha > 254)
		{
			dust.active = false;
		}
		dust.position += dust.velocity;
		dust.velocity *= 0.9f;
		return false;
	}
}
